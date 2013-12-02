using System;
using System.Runtime.Serialization;

namespace PICalculator.Model.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BusinessException : Exception
    {
        // constructors...
        #region BusinessException()
        /// <summary>
        /// Constructs a new BusinessException.
        /// </summary>
        public BusinessException() { }
        #endregion
        #region BusinessException(string message)
        /// <summary>
        /// Constructs a new BusinessException.
        /// </summary>
        /// <param name="message">The exception message</param>
        public BusinessException(string message) : base(message) { }
        #endregion
        #region BusinessException(string message, Exception innerException)
        /// <summary>
        /// Constructs a new BusinessException.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public BusinessException(string message, Exception innerException) : base(message, innerException) { }
        #endregion
        #region BusinessException(SerializationInfo info, StreamingContext context)
        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }    
}
