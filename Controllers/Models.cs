using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreElasticApp.Controllers
{
    public interface BaseDocument
    {
        int Id { get; set; }
        string Name { get; set; }

        JoinField joinField { get; set; }
    }

    [ElasticsearchType(IdProperty ="Id", RelationName ="Model1")]
    public class Model1: BaseDocument
    {
        [Number(Name = "Id")]
        public int Id { get; set; }
        
        [Text(Name = "Name")]
        public string Name { get; set; }

        public List<Model2> Model2 { get; set; }
        public JoinField joinField { get; set; }
    }

    [ElasticsearchType(IdProperty = "Id", RelationName = "Model2")]
    public class Model2 : BaseDocument
    {
        [Number(Name = "Id")]
        public int Id { get; set; }

        [Text(Name = "Name")]
        public string Name { get; set; }

        public List<Model3> Model3 { get; set; }

        public JoinField joinField { get; set; }
    }

    [ElasticsearchType(IdProperty = "Id", RelationName = "Model3")]
    public class Model3 : BaseDocument
    {
        [Number(Name = "Id")]
        public int Id { get; set; }

        [Text(Name = "Name")]
        public string Name { get; set; }

        public JoinField joinField { get; set; }
    }
}
