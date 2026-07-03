using Shared.DDD;
using SharedWithUI.Catalog.Dtos;
using SharedWithUI.SalesOrder.Enums;

namespace SalesOrder.Orders.Models;

public class SalesOrder : Aggregate<Guid>
{
    private readonly List<SalesOrderLine> _lines = new();

    private SalesOrder()
    {
    }

    public string Number { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid? PriceListId { get; private set; }
    public Guid? SourceQuotationId { get; private set; }
    public string? SalespersonId { get; private set; }
    public SalesInvoicingPolicy InvoicingPolicy { get; private set; } = SalesInvoicingPolicy.InvoiceDeliveredQuantity;
    public SalesOrderSourceType SourceType { get; private set; } = SalesOrderSourceType.Manual;
    public Guid? SourceDocumentId { get; private set; }
    public string? SourceDocumentNumber { get; private set; }
    public Guid? PaymentId { get; private set; }
    public Guid? AccountingDocumentId { get; private set; }
    public Guid? ZatcaEInvoiceId { get; private set; }

    public SalesOrderStatus Status { get; private set; }

    public DateTime OrderDate { get; private set; }
    public DateTime? DeliveryDate { get; private set; }
    public string? CustomerPurchaseOrderNumber { get; private set; }
    public string? Notes { get; private set; }
    public string? Terms { get; private set; }

    public decimal Subtotal { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal TotalAmount { get; private set; }

    public IReadOnlyCollection<SalesOrderLine> Lines => _lines;

    public bool IsCompleted =>
        Status == SalesOrderStatus.Completed;

    public bool IsCancelled =>
        Status == SalesOrderStatus.Cancelled;

    public bool FullyDelivered =>
    _lines.All(x => x.IsFullyDelivered);

    public bool FullyReserved =>
        _lines.Where(x => !x.IsDeleted && x.RemainingToDeliverQuantity > 0).All(x => x.IsFullyReserved);

    public bool HasAnyReservation =>
        _lines.Any(x => !x.IsDeleted && x.ReservedQuantity > 0);

    public bool FullyInvoiced =>
        _lines.All(x => x.IsFullyInvoiced);

    public DateTime? ConfirmedAt { get; private set; }
    public string? ConfirmedBy { get; set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancelledBy { get; set; }
    public DateTime? CompletedAt { get; private set; }

    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? StoreFrontId { get; private set; }
    public Guid? PosCashierSessionId { get; private set; }
    public static SalesOrder Create(
        Guid id,
        string number,
        Guid customerId,
        Guid? priceListId,
        Guid companyId,
        Guid? branchId,
        Guid? storeFrontId,
        Guid? posCashierSessionId,
        string createdBy,
        string? salespersonId = null,
        Guid? sourceQuotationId = null,
        SalesInvoicingPolicy invoicingPolicy = SalesInvoicingPolicy.InvoiceDeliveredQuantity,
        SalesOrderSourceType sourceType = SalesOrderSourceType.Manual,
        Guid? sourceDocumentId = null,
        string? sourceDocumentNumber = null,
        Guid? paymentId = null,
        DateTime? deliveryDate = null,
        string? customerPurchaseOrderNumber = null,
        string? notes = null,
        string? terms = null)
    {
        return new SalesOrder
        {
            Id = id,
            Number = number,
            CustomerId = customerId,
            PriceListId = priceListId,
            Status = SalesOrderStatus.Draft,
            OrderDate = DateTime.UtcNow,
            CompanyId = companyId,
            BranchId = branchId,
            StoreFrontId = storeFrontId,
            PosCashierSessionId = posCashierSessionId,
            SalespersonId = salespersonId ?? createdBy,
            SourceQuotationId = sourceQuotationId,
            InvoicingPolicy = invoicingPolicy,
            SourceType = sourceType,
            SourceDocumentId = sourceDocumentId,
            SourceDocumentNumber = sourceDocumentNumber,
            PaymentId = paymentId,
            DeliveryDate = deliveryDate,
            CustomerPurchaseOrderNumber = customerPurchaseOrderNumber,
            Notes = notes,
            Terms = terms,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update(
        Guid? priceListId,
        List<SalesOrderLine> updateLines,
        string modifiedBy)
    {
        EnsureDraft();
        PriceListId = priceListId;

        var activeValues = _lines.Where(v => !v.IsDeleted).ToList();
        var activeIds = activeValues.Select(v => v.Id).ToHashSet();

        // Add + Update
        foreach (var l in updateLines)
        {
            if (l.Id == Guid.Empty)
            {
                AddLine(
                    l.ProductId,
                    l.ProductSkuId,
                    l.ProductName,
                    l.ProductNameEng,
                    l.SkuCode,
                    l.Quantity,
                    l.UnitPrice,
                    l.UnitOfMeasureId,
                    l.DiscountRate,
                    l.TaxRate,
                    l.Notes,
                    modifiedBy);
                continue;
            }

            // 🚨 ONLY validate against ACTIVE values
            if (!activeIds.Contains(l.Id))
                throw new Exception($"Invalid or deleted Order items Id: {l.Id}");


            var existingValue = activeValues.First(ev => ev.Id == l.Id);
            existingValue.Update(l.Quantity, l.UnitPrice, l.UnitOfMeasureId, l.DiscountRate, l.TaxRate, l.Notes, modifiedBy);
        }

        // Remove
        var dtoIds = updateLines
            .Where(v => v.Id != Guid.Empty)
            .Select(v => v.Id)
            .ToHashSet();

        var valuesToRemove = activeValues
            .Where(ev => !dtoIds.Contains(ev.Id))
            .ToList();

        foreach (var value in valuesToRemove)
        {
            value.Remove(modifiedBy);
        }

        RecalculateTotals();
    }

    public void ApplyResolvedPriceList(Guid? priceListId)
    {
        EnsureDraft();
        PriceListId = priceListId;
    }

    public void LinkAccounting(Guid accountingDocumentId, Guid? zatcaEInvoiceId)
    {
        AccountingDocumentId = accountingDocumentId;
        ZatcaEInvoiceId = zatcaEInvoiceId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (!_lines.Any())
            throw new Exception("Order has no lines.");

        if (Status != SalesOrderStatus.Draft)
            throw new Exception("Only draft orders can be confirmed.");

        Status = SalesOrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }


    //public void Cancel()
    //public void MarkShipped()
    //public void MarkCompleted()
    public void Deliver(Dictionary<Guid, decimal> deliveredLines)
    {
        if (Status == SalesOrderStatus.Cancelled)
            throw new Exception("Order cancelled.");

        if (Status != SalesOrderStatus.Confirmed &&
            Status != SalesOrderStatus.PartiallyReserved &&
            Status != SalesOrderStatus.Reserved &&
            Status != SalesOrderStatus.Invoiced &&
            Status != SalesOrderStatus.PartiallyInvoiced &&
            Status != SalesOrderStatus.PartiallyDelivered)
            throw new Exception("Order cannot be delivered.");

        if (deliveredLines.Any())
        {

            foreach (var l in deliveredLines)
            {
                var line = GetLine(l.Key);
                line.Deliver(l.Value);

            }

            Status = FullyDelivered
                ? SalesOrderStatus.Delivered
                : SalesOrderStatus.PartiallyDelivered;
        }


        //AddDomainEvent(
        //    new SalesOrderDeliveredDomainEvent(
        //        Id,
        //        lineId,
        //        quantity));
    }

    public void Invoice(Dictionary<Guid, decimal> deliveredLines)
    {
        //if (Status != SalesOrderStatus.Delivered &&
        //    Status != SalesOrderStatus.PartiallyDelivered)
        //{
        //    throw new Exception("Order not delivered.");
        //}

        if (Status == SalesOrderStatus.Cancelled)
            throw new Exception("Cancelled order.");

        if (Status == SalesOrderStatus.Completed)
            throw new Exception("Completed order.");

        if (deliveredLines.Any())
        {

            foreach (var l in deliveredLines)
            {
                var line = GetLine(l.Key);
                if (InvoicingPolicy == SalesInvoicingPolicy.InvoiceDeliveredQuantity &&
                    line.InvoicedQuantity + l.Value > line.DeliveredQuantity)
                    throw new Exception("Cannot invoice more than delivered quantity.");

                line.Invoice(l.Value);

            }

            Status = FullyInvoiced
            ? SalesOrderStatus.Invoiced
            : SalesOrderStatus.PartiallyInvoiced;
        }


        
    }

    public void Complete()
    {
        if (!FullyDelivered)
            throw new Exception("Order not fully delivered.");
        //throw new DomainException("Order not fully delivered.");

        if (!FullyInvoiced)
            throw new Exception("Order not fully invoiced.");
        //throw new DomainException("Order not fully invoiced.");

        Status = SalesOrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        //AddDomainEvent(new SalesOrderCompletedDomainEvent(Id));
    }
    public void Cancel(string reason, string canceledBy)
    {
        if (Status == SalesOrderStatus.Completed)
            throw new Exception("Completed order cannot be cancelled.");
        //throw new DomainException("Completed order cannot be cancelled.");
        if (_lines.Any(x => x.DeliveredQuantity > 0))
            throw new Exception("Delivered order cannot be cancelled.");
        if (_lines.Any(x => x.ReservedQuantity > 0))
            throw new Exception("Reserved order cannot be cancelled. Release reservations first.");
        Status = SalesOrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancelledBy = canceledBy;
        //AddDomainEvent(
        //    new SalesOrderCancelledDomainEvent(
        //        Id,
        //        reason));
    }

    public SalesOrderLine AddLine(
        //int lineNumber,
        Guid productId,
        Guid skuId,
        string productName,
        string productNameEng,
        string skuCode,
        decimal quantity,
        decimal unitPrice,
        Guid unitId,
        decimal discountRate,
        decimal taxRate,
        string? notes,
        string createdBy)
    {
        EnsureDraft();
        //if (_lines.Any(x => x.ProductSkuId == skuId))
        //    throw new Exception("Product already exists.");

        var line = SalesOrderLine.Create(
            _lines.Count == 0 ? 1 : Lines.Max(x => x.LineNumber) + 1,
            productId,
            skuId,
            productName,
            productNameEng,
            skuCode,
            quantity,
            unitPrice,
            unitId,
            discountRate,
            taxRate,
            notes,
            createdBy);

        _lines.Add(line);

        RecalculateTotals();
        return line;
    }
    public void RemoveLine(Guid lineId)
    {
        EnsureDraft();

        var line = _lines.FirstOrDefault(x => x.Id == lineId);

        if (line == null)
            throw new Exception("Line not found.");
        //throw new DomainException("Line not found.");

        _lines.Remove(line);

        RecalculateTotals();
    }
    public void ChangeLineQuantity(Guid lineId, decimal quantity)
    {
        EnsureDraft();

        var line = GetLine(lineId);

        line.ChangeQuantity(quantity);

        RecalculateTotals();
    }
    public void ChangePrice(Guid lineId, decimal price)
    {
        EnsureDraft();

        var line = GetLine(lineId);

        line.ChangePrice(price);

        RecalculateTotals();
    }
    public void ChangeDiscount(Guid lineId, decimal discount)
    {
        EnsureDraft();

        var line = GetLine(lineId);

        line.ChangeDiscount(discount);

        RecalculateTotals();
    }
    public void ChangeTaxRate(Guid lineId, decimal tax)
    {
        EnsureDraft();

        var line = GetLine(lineId);

        line.ChangeTaxRate(tax);

        RecalculateTotals();
    }
    private void RecalculateTotals()
    {
        Subtotal = _lines.Sum(x => x.NetAmount);

        TaxAmount = _lines.Sum(x => x.TaxAmount);

        TotalAmount = Subtotal + TaxAmount;
    }

    private void EnsureDraft()
    {
        if (Status != SalesOrderStatus.Draft)
            throw new Exception("Order is not editable.");
        //throw new DomainException("Order is not editable.");
    }

    private SalesOrderLine GetLine(Guid lineId)
    {
        var line = _lines.FirstOrDefault(x => x.Id == lineId);

        if (line == null)
            throw new Exception("Line not found.");
        //throw new DomainException("Line not found.");

        return line;
    }

    public void ReserveLine(Guid lineId, decimal quantity)
    {
        EnsureReservable();
        var line = GetLine(lineId);
        line.Reserve(quantity);
        RefreshReservationStatus();
    }

    public void ReleaseLineReservation(Guid lineId, decimal quantity)
    {
        EnsureReservationEditable();
        var line = GetLine(lineId);
        line.ReleaseReservation(quantity);
        RefreshReservationStatus();
    }

    public void ConsumeLineReservation(Guid lineId, decimal quantity)
    {
        EnsureReservationEditable();
        var line = GetLine(lineId);
        line.ConsumeReservation(quantity);
        RefreshReservationStatus();
    }

    private void EnsureReservable()
    {
        if (Status != SalesOrderStatus.Confirmed &&
            Status != SalesOrderStatus.PartiallyReserved &&
            Status != SalesOrderStatus.Reserved)
            throw new Exception("Order cannot be reserved.");
    }

    private void EnsureReservationEditable()
    {
        if (Status == SalesOrderStatus.Cancelled || Status == SalesOrderStatus.Completed)
            throw new Exception("Order reservation cannot be changed.");
    }

    private void RefreshReservationStatus()
    {
        if (Status is SalesOrderStatus.Delivered or SalesOrderStatus.PartiallyDelivered or SalesOrderStatus.Invoiced or SalesOrderStatus.PartiallyInvoiced or SalesOrderStatus.Completed)
            return;

        if (!HasAnyReservation)
        {
            Status = SalesOrderStatus.Confirmed;
            return;
        }

        Status = FullyReserved ? SalesOrderStatus.Reserved : SalesOrderStatus.PartiallyReserved;
    }

    public void Return(Dictionary<Guid, decimal> returnedLines)
    {
        if (Status == SalesOrderStatus.Cancelled)
            throw new Exception("Cancelled order.");

        if (returnedLines.Any())
        {
            foreach (var l in returnedLines)
            {
                var line = GetLine(l.Key);
                line.Return(l.Value);
            }
        }
    }
}


