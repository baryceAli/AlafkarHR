using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.WebsiteInquiries;

public class CreateWebsiteInquiryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/Settings/public/inquiries", async (CreateWebsiteInquiryDto request, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new CreateWebsiteInquiryCommand(request), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("CreateWebsiteInquiry")
        .Produces<CreateWebsiteInquiryResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .AllowAnonymous();
    }
}
