using System.ComponentModel.DataAnnotations;

namespace TeleMedicine_BE.ViewModels
{
    public enum DrugFieldEnum
    {
        Id,
        Name,
        Producer,
        DrugOrigin,
        DrugForm,
        DrugTypeId
    }
    public class DrugVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Producer { get; set; }
        public string DrugOrigin { get; set; }
        public string DrugForm { get; set; }
        public virtual DrugTypeVM DrugType { get; set; }
        public int DrugTypeId { get; set; }
    }

    public class DrugCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Producer { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string DrugOrigin { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string DrugForm { get; set; }
        [Required]
        public int DrugTypeId { get; set; }
    }

    public class DrugUM
    {
        [Required]
        public int Id;
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Producer { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string DrugOrigin { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string DrugForm { get; set; }
        [Required]
        public int DrugTypeId { get; set; }
    }
}
