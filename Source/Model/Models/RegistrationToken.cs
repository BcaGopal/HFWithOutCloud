﻿using System;

namespace Model.Models
{
    public class RegistrationToken
    {
        public int RegistrationTokenId { get; set; }

        public Guid Token { get; set; }

        public string Role { get; set; }
    }
}
