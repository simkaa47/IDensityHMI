﻿namespace IDensity.DataAccess.Models
{
    public class MeasResult
    {
        public long Id { get; set; }

        public float Pulses { get; set; }
        public float CurValue1 { get; set; }
        public float AvgValue1 { get; set; }
        public float CurValue2 { get; set; }
        public float AvgValue2 { get; set; }
        public float HvValue1 { get; set; } 
        public float Current1 { get; set; }
        public float Current2 { get; set; }
        public float Temperature { get; set; }


    }
}