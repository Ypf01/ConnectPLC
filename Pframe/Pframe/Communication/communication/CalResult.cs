using System;

namespace Pframe
{
    public class CalResult
    {
        public bool IsSuccess
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public int ErrorCode
        {
            get;
            set;
        }

        public CalResult()
        {
            this.Message = "Unknown";
            this.ErrorCode = 10000;
        }

        public CalResult(string msg) : this()
        {
            this.Message = msg;
        }

        public CalResult(int err, string msg) : this(msg)
        {
            this.ErrorCode = err;
        }

        public void CopyErrorFromOther<TResult>(TResult result) where TResult : CalResult
        {
            if (result != null)
            {
                this.ErrorCode = result.ErrorCode;
                this.Message = result.Message;
            }
        }

        public static CalResult CreateFailedResult()
        {
            return new CalResult
            {
                IsSuccess = false,
                ErrorCode = 10000,
                Message = "Unknown"
            };
        }

        public static CalResult<T> CreateFailedResult<T>(CalResult result)
        {
            return new CalResult<T>
            {
                ErrorCode = result.ErrorCode,
                Message = result.Message
            };
        }

        public static CalResult<T1, T2> CreateFailedResult<T1, T2>(CalResult result)
        {
            return new CalResult<T1, T2>
            {
                ErrorCode = result.ErrorCode,
                Message = result.Message
            };
        }

        public static CalResult<T1, T2, T3> CreateFailedResult<T1, T2, T3>(CalResult result)
        {
            return new CalResult<T1, T2, T3>
            {
                ErrorCode = result.ErrorCode,
                Message = result.Message
            };
        }

        public static CalResult<T1, T2, T3, T4> CreateFailedResult<T1, T2, T3, T4>(CalResult result)
        {
            return new CalResult<T1, T2, T3, T4>
            {
                ErrorCode = result.ErrorCode,
                Message = result.Message
            };
        }

        public static CalResult<T1, T2, T3, T4, T5> CreateFailedResult<T1, T2, T3, T4, T5>(CalResult result)
        {
            return new CalResult<T1, T2, T3, T4, T5>
            {
                ErrorCode = result.ErrorCode,
                Message = result.Message
            };
        }

        public static CalResult CreateSuccessResult()
        {
            return new CalResult
            {
                IsSuccess = true,
                ErrorCode = 0,
                Message = "Success"
            };
        }

        public static CalResult<T> CreateSuccessResult<T>(T value)
        {
            return new CalResult<T>
            {
                IsSuccess = true,
                ErrorCode = 0,
                Message = "Success",
                Content = value
            };
        }

        public static CalResult<T1, T2> CreateSuccessResult<T1, T2>(T1 value1, T2 value2)
        {
            return new CalResult<T1, T2>
            {
                IsSuccess = true,
                ErrorCode = 0,
                Message = "Success",
                Content1 = value1,
                Content2 = value2
            };
        }

        public static CalResult<T1, T2, T3> CreateSuccessResult<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            return new CalResult<T1, T2, T3>
            {
                IsSuccess = true,
                ErrorCode = 0,
                Message = "Success",
                Content1 = value1,
                Content2 = value2,
                Content3 = value3
            };
        }

        public static CalResult<T1, T2, T3, T4> CreateSuccessResult<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            return new CalResult<T1, T2, T3, T4>
            {
                IsSuccess = true,
                ErrorCode = 0,
                Message = "Success",
                Content1 = value1,
                Content2 = value2,
                Content3 = value3,
                Content4 = value4
            };
        }

        public static CalResult<T1, T2, T3, T4, T5> CreateSuccessResult<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            return new CalResult<T1, T2, T3, T4, T5>
            {
                IsSuccess = true,
                ErrorCode = 0,
                Message = "Success",
                Content1 = value1,
                Content2 = value2,
                Content3 = value3,
                Content4 = value4,
                Content5 = value5
            };
        }
    }
    public class CalResult<T> : CalResult
    {
        public T Content
        {
            get;
            set;
        }

        public CalResult()
        {
            this.Message = "Unknown";
            this.ErrorCode = 10000;
        }

        public CalResult(string msg) : this()
        {
            this.Message = msg;
        }

        public CalResult(int err, string msg) : this(msg)
        {
            this.ErrorCode = err;
        }
    }
    public class CalResult<T1, T2> : CalResult
    {
        public T1 Content1
        {
            get;
            set;
        }

        public T2 Content2
        {
            get;
            set;
        }

        public CalResult() : base()
        {
        }

        public CalResult(string msg) : base(msg)
        {
        }

        public CalResult(int err, string msg) : base(err, msg)
        {
        }
    }
    public class CalResult<T1, T2, T3> : CalResult
    {
        public T1 Content1
        {
            get;
            set;
        }

        public T2 Content2
        {
            get;
            set;
        }

        public T3 Content3
        {
            get;
            set;
        }

        public CalResult() : base()
        {
        }

        public CalResult(string msg) : base(msg)
        {
        }

        public CalResult(int err, string msg) : base(err, msg)
        {
        }
    }
    public class CalResult<T1, T2, T3, T4> : CalResult
    {
        public T1 Content1
        {
            get;
            set;
        }

        public T2 Content2
        {
            get;
            set;
        }

        public T3 Content3
        {
            get;
            set;
        }

        public T4 Content4
        {
            get;
            set;
        }
        public CalResult() : base()
        {
        }

        public CalResult(string msg) : base(msg)
        {
        }

        public CalResult(int err, string msg) : base(err, msg)
        {
        }
    }
    public class CalResult<T1, T2, T3, T4, T5> : CalResult
    {
        public T1 Content1
        {
            get;
            set;
        }

        public T2 Content2
        {
            get;
            set;
        }

        public T3 Content3
        {
            get;
            set;
        }

        public T4 Content4
        {
            get;
            set;
        }

        public T5 Content5
        {
            get;
            set;
        }

        public CalResult() : base()
        {
        }

        public CalResult(string msg) : base(msg)
        {
        }

        public CalResult(int err, string msg) : base(err, msg)
        {
        }
    }
}
