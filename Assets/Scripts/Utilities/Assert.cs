﻿using System;

namespace Utilities
{
    public class AssertException : Exception
    {
        public AssertException()
        {
        }

        public AssertException(string message) : base(message)
        {
            
        }
    }
    
    public static class Assert
    {
        public static void True(bool value, string message = default)
        {
#if UNITY_EDITOR
            if (!value)
            {
                if (message != default)
                {
                    throw new AssertException(message);
                }
                throw new AssertException();
            }
#endif
        }

        public static void NotNull(object obj, string message = default)
        {
#if UNITY_EDITOR
            if (obj.IsUnityNull())
            {
                if (message != default)
                {
                    throw new AssertException(message);
                }
                throw new AssertException();
            }
#endif
        }

        public static void Null(object obj, string message = default)
        {
#if UNITY_EDITOR
            if (!obj.IsUnityNull())
            {
                if (message != default)
                {
                    throw new AssertException(message);
                }
                throw new AssertException();
            }
#endif
        }
    }
}