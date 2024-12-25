using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using LibraryMangamentSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using LibraryMangamentSystem.Model.Respository;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryMangamentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IBookRepo _bookRepo;
        private ResponseDTO _responseAPI;
        private readonly IBorrowRepo _borrowRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public UserController(IMapper mapper, IConfiguration configuration, IUserRepo userRepo, IBookRepo bookRepo, IBorrowRepo borrowRepo)
        {
            _responseAPI = new ResponseDTO();
            _mapper = mapper;
            _configuration = configuration;
            _userRepo = userRepo;
            _bookRepo = bookRepo;
            _borrowRepo = borrowRepo;
        }

        [HttpGet]
        [Route("GetALL")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "admin,User")]
        public async Task<ActionResult<ResponseDTO>> GetAllAsync()
        {
            try
            {
                var books = await _bookRepo.getAll();

                if (books == null || !books.Any())
                    return NotFound(new { Message = "No Data Exists" });
                _responseAPI.Data = books;
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

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseDTO>> CreateUserAsync([FromBody] UserDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { Message = "Bad Request" });

                // Check if BorrowRecords is null or empty
                if (model.BorrowRecords != null && model.BorrowRecords.Any())
                {
                    foreach (var record in model.BorrowRecords)
                    {
                        var bookExists = await _bookRepo.getUser(b => b.BookID == record.BookId);
                        if (bookExists == null)
                            return BadRequest(new { Message = $"Book with ID {record.BookId} does not exist." });
                    }
                }

                // Map and save user
                var uuser = _mapper.Map<User>(model);
                uuser.BorrowRecords = model.BorrowRecords?.Any() == true ? _mapper.Map<List<BorrowRecords>>(model.BorrowRecords) : null;

                await _userRepo.create(uuser);

                // Map response
                var responseUserDTO = _mapper.Map<UserDTO>(uuser);
                _responseAPI.Data = responseUserDTO;
                _responseAPI.status = true;
                _responseAPI.statusCode = HttpStatusCode.OK;

                return _responseAPI;
            }
            catch (Exception ex)
            {
                _responseAPI.Errors.Add(ex.InnerException?.Message ?? ex.Message);
                _responseAPI.statusCode = HttpStatusCode.InternalServerError;
                _responseAPI.status = false;
                return _responseAPI;
            }
        }

        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO>> LoginUser([FromBody] LoginDTO model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.mail) || string.IsNullOrEmpty(model.password))
                {
                    return BadRequest(new { Message = "Please provide both email and password." });
                }
                var Roole = "";
                var uuser = await _userRepo.getUser(x => x.Email == model.mail);
                if (uuser.Role == model.role)
                    Roole = model.role;
                else
                {
                    return BadRequest(new { Message = "Please provide Valid Role" });
                }
                if (uuser == null || uuser.Password != model.password)
                {
                    return Unauthorized(new { Message = "Invalid email or password." });
                }

                var responseUserDTO = _mapper.Map<UserDTO>(uuser);

                var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, model.mail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, model.role)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                var ttoken = new JwtSecurityTokenHandler().WriteToken(token);

                _responseAPI.Data = new { Token = ttoken, User = responseUserDTO };
                _responseAPI.status = true;
                _responseAPI.statusCode = HttpStatusCode.OK;

                return _responseAPI;
            }
            catch (Exception ex)
            {
                _responseAPI.Errors.Add(ex.InnerException?.Message ?? ex.Message);
                _responseAPI.statusCode = HttpStatusCode.InternalServerError;
                _responseAPI.status = false;
                return _responseAPI;
            }
        }

        [HttpPost]
        [Route("borrow")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "admin,User")]
        public async Task<ActionResult<ResponseDTO>> Borrow([FromBody]BorrowRequest model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Put Valid Data ");

                var exsistBook = await _bookRepo.getUser(x => x.BookID == model.BookId);
                if(exsistBook == null)
                    return NotFound("Book not exsist with this ID ");

                var uuser = await _userRepo.getUser(x => x.Id == model.UserId);
                if (uuser == null)
                    return NotFound("User not exsist with this ID ");

                if (!exsistBook.IsAvailable)
                    return BadRequest("Book is already lent out by another user.");

                var data = _mapper.Map<BorrowRecords>(model);
                await _borrowRepo.create(data);

                exsistBook.IsAvailable = false;
                _bookRepo.update(exsistBook);

                _responseAPI.status = true;
                _responseAPI.statusCode = HttpStatusCode.OK;
                _responseAPI.Data = "Borrow Successful";
                _responseAPI.Errors = null;
                return _responseAPI;
            }
            catch (Exception ex)
            {
                _responseAPI.Errors.Add(ex.InnerException?.Message ?? ex.Message);
                _responseAPI.statusCode = HttpStatusCode.InternalServerError;
                _responseAPI.status = false;
                return _responseAPI;
            }

        }

        [HttpPost]
        [Route("Return")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "admin,User")]
        public async Task<ActionResult<ResponseDTO>> Return([FromBody] BorrowRequest model) 
        {
            try
            {
                if (model == null)
                    return BadRequest("Put Valid Data ");
                var exsistBook = await _bookRepo.getUser(x => x.BookID == model.BookId);
                if (exsistBook == null)
                    return NotFound("Book not exsist with this ID ");

                var uuser = await _userRepo.getUser(x => x.Id == model.UserId);
                if (uuser == null)
                    return NotFound("User not exsist with this ID ");

                var borrowRecord = await _borrowRepo.getUser(x => x.BookId == model.BookId && x.UserId == model.UserId);
                if (borrowRecord == null)
                    return NotFound("No borrowing record found for this book and user.");
                await _borrowRepo.delete(borrowRecord);

                exsistBook.IsAvailable = true;
                _bookRepo.update(exsistBook);

                _responseAPI.status = true;
                _responseAPI.statusCode = HttpStatusCode.OK;
                _responseAPI.Data = "Book Returned !!!";
                _responseAPI.Errors = null;
                return _responseAPI;
            }
            catch (Exception ex) 
            {
                _responseAPI.Errors.Add(ex.InnerException?.Message ?? ex.Message);
                _responseAPI.statusCode = HttpStatusCode.InternalServerError;
                _responseAPI.status = false;
                return _responseAPI;
            }
        }
    }
}
