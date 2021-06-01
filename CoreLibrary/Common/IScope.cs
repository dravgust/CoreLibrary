﻿using System;

namespace CoreLibrary.Common
{
    /// <summary>
    /// Represents object lifetime scope
    /// </summary>
    /// <typeparam name="T">instance type</typeparam>
    public interface IScope<out T> : IDisposable
    {
        T Instance { get; }
    }
}
