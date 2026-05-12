namespace RestflowAPI.DTOs.Customers
{
	public class CustomerResponseDto
	{
		public bool IsSuccess { get; set; }
		public string Message { get; set; } = string.Empty;
		public CustomerDto? Data { get; set; }

		public static CustomerResponseDto Success(CustomerDto data, string message = "Success")
		{
			return new CustomerResponseDto
			{
				IsSuccess = true,
				Message = message,
				Data = data
			};
		}

		public static CustomerResponseDto Failure(string message)
		{
			return new CustomerResponseDto
			{
				IsSuccess = false,
				Message = message
			};
		}
	}
}
