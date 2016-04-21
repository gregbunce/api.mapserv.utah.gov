﻿namespace WebAPI.Domain.Linkers
{
    public abstract class GridLinkable : IWeighable
    {
        protected GridLinkable(string grid, int weight)
        {
            Grid = grid;
            Weight = weight;
        }

        public abstract string Key { get; }
        public string Grid { get; set; }
        public int Weight { get; set; }

        public override string ToString()
        {
            return string.Format("Grid: {0}", Grid);
        }
    }
}