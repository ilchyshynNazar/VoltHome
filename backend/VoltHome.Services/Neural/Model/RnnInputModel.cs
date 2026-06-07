using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Services.Neural.Model;

public class RnnInputModel
{
    public float[] Sequence { get; set; } = null!; 
    public float Label { get; set; }              
}
