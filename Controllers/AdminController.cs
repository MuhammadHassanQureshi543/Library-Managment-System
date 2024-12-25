using System.Net;
using AutoMapper;
using LibraryMangamentSystem.Model;
using LibraryMangamentSystem.Model.Respository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryMangamentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="admin")]
    public class AdminController : ControllerBase
    {
        private ResponseDTO _responseAPI;
        private readonly IMapper _mapper;
        private readonly IBookRepo _bookRepo;
        public AdminController(IMapper mapper, IBookRepo bookRepo)
        {
            _mapper = mapper;
            _bookRepo = bookRepo;
            _responseAPI = new ResponseDTO();
        }

        [HttpPost]
        [Route("CreateBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO>> AddBook([FromBody]BooksDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Fill the All fields");
                var Book = _mapper.Map<Books>(model);
                await _bookRepo.create(Book);
                var responseDTO = _mapper.Map<BooksDTO>(Book);
                _responseAPI.Data = responseDTO;
                _responseAPI.status = true;
                _responseAPI.statusCode = HttpStatusCode.OK;
                return _responseAPI;
            }
            catch (Exception ex)
            {
                _responseAPI.Errors.Add(ex.InnerException?.Message ?? ex.Message);
                _responseAPI.statusCode = HttpStatusCode.InternalServerError;
                _responseAPI.status = false;
                return StatusCode((int)_responseAPI.statusCode, _responseAPI);
            }
        }

        [HttpPut]
        [Route("UpdateBook/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO>> UpdateBook(int id, [FromBody] BooksDTO model)
        {
            try
            {
                if (model == null || id<=0)
                    return BadRequest("Enter Valid Data");
                var bBook = await _bookRepo.getUser(x=>x.BookID ==id);
                if (bBook == null)
                {
                    _responseAPI.Errors.Add("Book not found.");
                    _responseAPI.statusCode = HttpStatusCode.NotFound;
                    _responseAPI.status = false;
                    return _responseAPI;
                }

                bBook.Title = model.Title;
                bBook.Author = model.Author;
                bBook.Category = model.Category;
                bBook.IsAvailable = model.IsAvailable;

                await _bookRepo.update(bBook);

                var updatedBookDTO = _mapper.Map<BooksDTO>(bBook);
                _responseAPI.Data = updatedBookDTO;
                _responseAPI.status = true;
                _responseAPI.statusCode = HttpStatusCode.OK;

                return _responseAPI;
            }
            catch(Exception ex)
            {
                _responseAPI.Errors.Add(ex.Message);
                _responseAPI.statusCode = HttpStatusCode.InternalServerError;
                _responseAPI.status = false;
                return _responseAPI;
            }
        }

        [HttpDelete]
        [Route("DelteBook/{id:int}")]
        public async Task<ActionResult<ResponseDTO>> DelteBook(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Id must be valid");

                var bBook = await _bookRepo.getUser(x => x.BookID == id);

                if (bBook == null)
                {
                    _responseAPI.Errors.Add("Book not found.");
                    _responseAPI.statusCode = HttpStatusCode.NotFound;
                    _responseAPI.status = false;
                    return _responseAPI;
                }

                _bookRepo.delete(bBook);

                var line = "Delte Successful";
                _responseAPI.Data = line;
                _responseAPI.status = true;
                _responseAPI.statusCode = HttpStatusCode.OK;

                return _responseAPI;
            }
            catch (Exception ex) 
            {
                _responseAPI.Errors.Add(ex.Message);
                _responseAPI.statusCode = HttpStatusCode.InternalServerError;
                _responseAPI.status = false;
                return _responseAPI;
            }
        }
    }
}
