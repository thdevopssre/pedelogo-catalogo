using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PedeLogo.Catalogo.Api.Model
{
    public class Servidor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
    }
}