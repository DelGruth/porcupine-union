namespace UserAccessSystem.Contract
{
    public struct Response<T>
    {
        public bool Success { get; set; }
        public ErrorCode ErrorCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public Response(T data)
        {
            this.Data = data;
            this.Success = true;
            this.ErrorCode = ErrorCode.None;
            this.Message = "Much Success";
        }

        public Response(ErrorCode errorCode, string message)
        {
            this.Success = false;
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Data = default(T);
        }

        public Response(ErrorCode errorCode)
        {
            this.Success = false;
            this.ErrorCode = errorCode;
            this.Message = string.Empty;
            this.Data = default(T);
        }
    }
}
