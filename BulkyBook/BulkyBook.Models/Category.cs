using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BulkyBook.Models
{
    public class Category
    {
        [Key]
        [Name("テスト")]
        [Range(30,40)]
        public int Id { get; set; }
        
        [Display(Name="Category Name")]
        [Required]
        [MaxLength(50)]
        [Name("テスト名")]
        public string Name { get; set; }
    }
}
