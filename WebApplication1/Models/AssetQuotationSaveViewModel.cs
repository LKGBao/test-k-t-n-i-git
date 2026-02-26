using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AssetQuotationItemInput
    {
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }
    }

    public class AssetQuotationSaveViewModel
    {
        public int RequestId { get; set; }

        public int BranchId { get; set; }

        public int? SupplierId { get; set; }

        public string? SupplierName { get; set; }

        public List<AssetQuotationItemInput> QuotationItems { get; set; } = new();
    }
}

