﻿using System.ComponentModel.DataAnnotations;

namespace LibraryMangamentSystem.Model
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public List<BorrowRecordDTO> BorrowRecords { get; set; }
    }
}
