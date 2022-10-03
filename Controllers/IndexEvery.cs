using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreElasticApp.Controllers
{
    public class IndexEvery
    {
        public static readonly ElasticClient client = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")).EnableDebugMode());
        public IndexEvery()
        {

        }

        public async Task<CreateIndexResponse> CreateIndex()
        {
            return await client.Indices.CreateAsync("index", c => c
                .Map<BaseDocument>(m => m
                    .RoutingField(r => r.Required())
                    .AutoMap<Model1>()
                    .AutoMap<Model2>()
                    .AutoMap<Model3>()
                    .Properties(props => props
                        .Join(j => j
                            .Name(p => p.joinField)
                            .Relations(r => r
                                .Join<Model1, Model2>()
                                .Join<Model2, Model3>()
                            )
                        )
                    )
                )
            );
        }

        public List<Model1> BuildData()
        {
            List<Model1> model1 = new List<Model1>();
            List<Model2> model2 = new List<Model2>();
            List<Model3> model3 = new List<Model3>();

            model1.Add(new Model1 { Id = 1, Name = "A", joinField = JoinField.Root<Model1>() });

            model2.Add(new Model2 { Id = 3, Name = "c", joinField = JoinField.Link<Model2, Model1>(model1[0]) });
            model2.Add(new Model2 { Id = 4, Name = "D", joinField = JoinField.Link<Model2, Model1>(model1[0]) });

            model1[0].Model2 = model2;

            model3.Add(new Model3 { Id = 5, Name = "E", joinField = JoinField.Link<Model3, Model2>(model2[0]) });
            model3.Add(new Model3 { Id = 6, Name = "F", joinField = JoinField.Link<Model3, Model2>(model2[0]) });
            model3.Add(new Model3 { Id = 7, Name = "G", joinField = JoinField.Link<Model3, Model2>(model2[1]) });
            model3.Add(new Model3 { Id = 8, Name = "H", joinField = JoinField.Link<Model3, Model2>(model2[1]) });

            model1[0].Model2[0].Model3 = model3.Where(x => x.Id == 5 || x.Id == 6).ToList();
            model1[0].Model2[1].Model3 = model3.Where(x => x.Id == 7 || x.Id == 8).ToList();

            return model1;
        }

        public async Task BulkAsync()
        {
            var model = BuildData();

            var res = await client.BulkAsync(b => b
                .Index("index")
                .IndexMany<Model1>(model, (d, model1) => d
                    .Routing(model1.Id)
                    .Id(model1.Id))).ContinueWith(async c1 => {
                        if (c1.Result.Errors)
                        {
                            throw c1.Result.OriginalException;
                        }

                        foreach(Model1 mod in model)
                        {
                            await client.BulkAsync(b => b
                                .Index("index")
                                .IndexMany<Model2>(mod.Model2, (d, mode2) => d
                                    .Routing(mod.Id)
                                    .Id(mode2.Id))).ContinueWith(async c2 => {
                                        if (c2.Result.Errors)
                                        {
                                            throw c2.Result.OriginalException;
                                        }

                                        foreach (Model2 mod3 in mod.Model2)
                                        {
                                            await client.BulkAsync(b => b
                                                .Index("index")
                                                .IndexMany<Model3>(mod3.Model3, (d, mode3) => d
                                                    .Routing(mod.Id)
                                                    .Id(mode3.Id)));
                                        }
                                    });
                        }
                    });

            res.Wait();
        }
    }
}
