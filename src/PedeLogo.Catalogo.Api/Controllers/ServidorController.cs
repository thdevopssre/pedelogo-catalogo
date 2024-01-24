using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using PedeLogo.Catalogo.Api.Model;

namespace PedeLogo.Catalogo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServidorController : ControllerBase
    {
        private readonly ILogger<ServidorController> _logger;
        private readonly IMongoCollection<Servidor> _collection;

        public ServidorController(ILogger<ServidorController> logger, IMongoDatabase mongoDB)
        {
            this._logger = logger;
            this._collection = mongoDB.GetCollection<Servidor>("Servidor");
        }
        
        [HttpGet]
        public IEnumerable<Servidor> Get()
        {
            var resultado = this._collection.Find(new BsonDocument()).ToList();
            this._logger.LogInformation("Entrou no Get All");
            return resultado;
        }

        [HttpGet("{id}", Name = "GetServidor")]
        public Servidor Get(string id)
        {
            this._logger.LogInformation("Entrou no Get By Id");
            ObjectId objID;
            if (!ObjectId.TryParse(id, out objID))
            {
                throw new Exception("Erro ao converter.");
            }

            return this._collection.Find(p => p.Id.Equals((id))).FirstOrDefault();
        }

        [HttpPost]
        public IActionResult Post([FromBody] Servidor servidor)
        {
            this._logger.LogInformation("Entrou no Post");
            this._collection.InsertOne(servidor);
            return Ok();
        }

        [HttpPut("{id}")]
        public void Put([FromRoute] string id, [FromBody] Servidor servidor)
        {
            this._logger.LogInformation("Entrou no Put");
            ObjectId objID;
            if (!ObjectId.TryParse(id, out objID))
            {
               throw new Exception("Id errado");
            }
            
            this._collection.FindOneAndReplace(obj => obj.Id.Equals(servidor.Id), servidor);
        }
        
        [HttpDelete]
        public void Delete(string id)
        {
            this._logger.LogInformation("Entrou no Delete");
            this._collection.FindOneAndDelete(obj => obj.Id.Equals(id));
        }
    }
}