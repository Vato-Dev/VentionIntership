namespace LibraryManager.Domain.ValueObjects
{
    public readonly record struct Isbn
    {
        public string Value { get; }
        private Isbn(string value)
        {
            Value = value;
        }
        public static Isbn Create(string value)
        {
            Validate(value);
            var cleanValue = value.Replace("-", "").Replace(" ", "").ToUpper();
            return new Isbn(cleanValue);
        }
        public static implicit operator string(Isbn isbn) => isbn.Value;
        

        private static void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) 
                throw new Exception("Value cannot be null or empty.");

            value = value.Replace("-", "").Replace(" ", "");

            if (value.Length == 13)
            {
                ValidateAndCheckValueForIsbn13(value);
                return; 
            }
            if (value.Length == 10)
            {
                ValidateAndCheckValueForIsbn10(value);
                return;
            }

            throw new Exception("Invalid ISBN length.");
        }

        private static void ValidateAndCheckValueForIsbn13(string value)
        {
            if (!value.All(char.IsDigit)) 
                throw new Exception("ISBN-13 must contain only digits.");

            if (value[..3] is not ("978" or "979"))
                throw new Exception("The ISBN prefix is invalid.");

            int totalSum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = value[i] - '0';
                totalSum += digit * (i % 2 == 0 ? 1 : 3);
            }

            int remainder = totalSum % 10;
            int checkDigit = (10 - remainder) % 10;

            if ((value[12] - '0') != checkDigit)
                throw new Exception("Invalid ISBN-13 check digit.");
        }
        private static void ValidateAndCheckValueForIsbn10(string value)
        {
            if (!value[..9].All(char.IsDigit)) 
                throw new Exception("First 9 characters of ISBN-10 must be digits.");

            int totalSum = 0;
    
            for (int i = 0; i < 9; i++)
            {
                int digit = value[i] - '0';
                totalSum += digit * (10 - i);
            }

            char lastChar = char.ToUpper(value[9]); 
            int lastDigitValue = lastChar == 'X' ? 10 : (lastChar - '0');

            if (lastChar != 'X' && !char.IsDigit(lastChar))
                throw new Exception("Invalid ISBN-10 check digit character.");

            totalSum += lastDigitValue;

            if (totalSum % 11 != 0) 
                throw new Exception("Invalid ISBN-10 check digit.");
        }

    }
}