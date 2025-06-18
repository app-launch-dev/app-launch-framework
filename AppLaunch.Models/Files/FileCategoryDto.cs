using System.ComponentModel.DataAnnotations;

namespace Applaunch.Models.Files;

public class FileCategoryDto
{
    [StringLength(75)]
    public string Category { get; set; }
}