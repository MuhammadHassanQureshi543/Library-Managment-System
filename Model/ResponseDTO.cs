using System.Net;

namespace LibraryMangamentSystem.Model
{
    public class ResponseDTO
    {
        public bool status {  get; set; }
        public HttpStatusCode statusCode { get; set; }
        public dynamic Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
