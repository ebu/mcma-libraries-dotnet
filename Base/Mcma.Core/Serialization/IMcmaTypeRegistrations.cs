﻿using System;

namespace Mcma.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMcmaTypeRegistrations
    {
        IMcmaTypeRegistrations Add<T>();

        IMcmaTypeRegistrations Add(Type type);
    }
}