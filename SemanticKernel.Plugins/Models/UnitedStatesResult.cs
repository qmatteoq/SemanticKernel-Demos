namespace SemanticKernel.Plugins.Models
{

    public class UnitedStatesResult
    {
        public PopulationResults[] data { get; set; }
        public Source[] source { get; set; }
    }

    public class PopulationResults
    {
        public string IDNation { get; set; }
        public string Nation { get; set; }
        public int IDYear { get; set; }
        public string Year { get; set; }
        public int Population { get; set; }
        public string SlugNation { get; set; }
    }

    public class Source
    {
        public string[] measures { get; set; }
        public Annotations annotations { get; set; }
        public string name { get; set; }
        public object[] substitutions { get; set; }
    }

    public class Annotations
    {
        public string source_name { get; set; }
        public string source_description { get; set; }
        public string dataset_name { get; set; }
        public string dataset_link { get; set; }
        public string table_id { get; set; }
        public string topic { get; set; }
        public string subtopic { get; set; }
    }

}
