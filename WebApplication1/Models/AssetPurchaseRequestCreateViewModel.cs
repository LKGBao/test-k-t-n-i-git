using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AssetPurchaseRequestItemInput
    {
        [Required]
        public int Asset_Category_Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }
    }

    public class AssetPurchaseRequestCreateViewModel
    {
        public int BranchId { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        public string? Note { get; set; }

        public List<AssetPurchaseRequestItemInput> Items { get; set; } = new();
    }

    public class AssetPurchaseRequestEditViewModel
    {
        public int Id { get; set; }

        public int BranchId { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? Note { get; set; }

        public List<AssetPurchaseRequestItemInput> Items { get; set; } = new();
    }
}

