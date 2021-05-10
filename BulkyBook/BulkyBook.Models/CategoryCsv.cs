using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BulkyBook.Models
{
    public class CategoryCsv
    {
        [Name("テスト")]
        public int Id { get; set; }
        

        [Name("テスト名")]
        public string Name { get; set; }

        [Name("結果")]
        public string Result { get; set; }
    }
}
