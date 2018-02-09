﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using Serilog;

namespace WebAPI.Common.Abstractions
{
    /// <summary>
    ///     A class with no return value
    /// </summary>
    public abstract class Command
    {
        public string ErrorMessage { get; set; }

        public void Run()
        {
            try
            {
                var timer = new Stopwatch();
                timer.Start();
                Execute();
                timer.Stop();
                Debug.Print("{0}-{1} Duration: {2}ms", ToString(), "Done",
                    timer.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
            }
            catch (AggregateException ex)
            {
                Log.Error("Geocoding error occurred.", ex);
                var errorList = new List<string>();

                foreach (var e in ex.Flatten().InnerExceptions)
                {
                    if (e is GeocodingException)
                    {
                        throw ex;
                    }

                    errorList.Add(e.Message);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Debug.Print("Error processing task:" + ToString(), ex);
                Debug.WriteLine("Error processing task:" + ToString(), ex.Message);
                Log.Fatal(ex, "Error processing task: {Task}", ToString());
            }
        }

        public abstract override string ToString();

        protected abstract void Execute();
    }

    /// <summary>
    ///     A command with a return value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Command<T> : Command
    {
        public T Result { get; protected set; }

        public T GetResult()
        {
            Run();
            return Result;
        }
    }
}
