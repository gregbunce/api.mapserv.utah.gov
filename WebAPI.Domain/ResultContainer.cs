﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Domain
{
    /// <summary>
    ///     A container class for returning api call results with status messages.
    /// </summary>
    public class ResultContainer
    {
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        public bool ShouldSerializeMessage()
        {
            return !string.IsNullOrEmpty(Message);
        }
    }

    /// <summary>
    ///     Generic ResultContainer for passing result data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultContainer<T> : ResultContainer where T : class
    {
        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }

        public bool ShouldSerializeResult()
        {
            if (Result is null)
            {
                return false;
            }

            return string.IsNullOrEmpty(Message);
        }
    }
}