﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DerDieDas.Models
{
    public enum MenuItemType
    {
        Worten,
        Verben
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
