using Logic.Enums;

namespace Logic.Models;

public class ConceptRelation
{
    public int id { get; set; }
    public string db_name { get; set; }
    public string view_name { get; set; }
    public RelationType type { get; set; }
}
