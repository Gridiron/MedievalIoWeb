﻿using System;

namespace MedievalIo.Server.Client.Models.News
{
    public class NewsEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageLink { get; set; }
    }
}
