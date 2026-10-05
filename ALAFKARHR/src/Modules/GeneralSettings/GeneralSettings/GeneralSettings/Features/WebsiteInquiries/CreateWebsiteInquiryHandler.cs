using FluentValidation;
using GeneralSettings.Data;
using GeneralSettings.GeneralSettings.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Catalog.Contracts.Products.Features.ValidatePublicWebsiteProduct;
using Organization.Contracts.Companies.Features.GetCompanyAccessStatus;
using Shared.Contracts.CQRS;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.WebsiteInquiries;

public record CreateWebsiteInquiryCommand(CreateWebsiteInquiryDto Inquiry) : ICommand<CreateWebsiteInquiryResult>;
public record CreateWebsiteInquiryResult(Guid InquiryId);

public class CreateWebsiteInquiryValidator : AbstractValidator<CreateWebsiteInquiryCommand>
{
    public CreateWebsiteInquiryValidator()
    {
        RuleFor(x => x.Inquiry).NotNull();
        When(x => x.Inquiry is not null, () =>
        {
            RuleFor(x => x.Inquiry.Name).NotEmpty().MinimumLength(2).MaximumLength(150).Must(x => x is not null && x.Trim().Length >= 2);
            RuleFor(x => x.Inquiry.Email).NotEmpty().EmailAddress().MaximumLength(254);
            RuleFor(x => x.Inquiry.Mobile).NotEmpty().MinimumLength(7).MaximumLength(30).Matches(@"^\+?[0-9\s()\-]{7,30}$");
            RuleFor(x => x.Inquiry.Message).NotEmpty().MinimumLength(10).MaximumLength(4000).Must(x => x is not null && x.Trim().Length >= 10);
            RuleFor(x => x.Inquiry.RequestType).Must(WebsiteInquiryOptions.Types.Contains);
            RuleFor(x => x.Inquiry.ContextKey).Must(x => x is null || WebsiteInquiryOptions.Contexts.Contains(x));
            RuleFor(x => x.Inquiry.SourcePage).Must(WebsiteInquiryOptions.Sources.Contains);
            RuleFor(x => x.Inquiry.Website).Must(string.IsNullOrEmpty).WithMessage("Invalid request.");
            RuleFor(x => x.Inquiry.ProductSkuId).NotNull().NotEqual(Guid.Empty).When(x => x.Inquiry.RequestType == "Product");
            RuleFor(x => x.Inquiry.ProductSkuId).Null().When(x => x.Inquiry.RequestType != "Product");
            RuleFor(x => x.Inquiry.ContextKey).NotEmpty().When(x => x.Inquiry.RequestType == "Package");
        });
    }
}

public class CreateWebsiteInquiryHandler(GeneralSettingsDbContext db, IConfiguration configuration, ISender sender)
    : ICommandHandler<CreateWebsiteInquiryCommand, CreateWebsiteInquiryResult>
{
    public async Task<CreateWebsiteInquiryResult> Handle(CreateWebsiteInquiryCommand request, CancellationToken cancellationToken)
    {
        // Company scope is resolved on the server; never accept a tenant ID from an anonymous visitor.
        if (!Guid.TryParse(configuration["PublicWebsite:CompanyId"], out var companyId) || companyId == Guid.Empty)
            throw new ValidationException("Public website company is not configured.");
        var company = await sender.Send(new GetCompanyAccessStatusQuery(companyId), cancellationToken);
        if (!company.CanLogin) throw new ValidationException("Public website company is unavailable.");
        if (request.Inquiry.ProductSkuId is Guid productId)
        {
            var product = await sender.Send(new ValidatePublicWebsiteProductQuery(companyId, productId), cancellationToken);
            if (!product.IsValid) throw new ValidationException("This product is not available for inquiry.");
        }
        var inquiry = WebsiteInquiry.Create(companyId, request.Inquiry);
        db.WebsiteInquiries.Add(inquiry);
        await db.SaveChangesAsync(cancellationToken);
        return new(inquiry.Id);
    }
}
