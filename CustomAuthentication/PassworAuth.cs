using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomAuthentication
{
    public class PassworAuth
    {
        public string base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }
        public string base64Decode2(string sData)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(sData);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }
        public bool ValidatePwdConfPwd(string pwd, string cnfPwd)
        {
            if (!string.IsNullOrWhiteSpace(pwd) && !string.IsNullOrWhiteSpace(cnfPwd)) throw new Exception("Password and Confirm password are required and it cannot be null or empty.");
            else if (!string.IsNullOrWhiteSpace(pwd)) throw new Exception("Password is required and it cannot be null or empty.");
            else if (!string.IsNullOrWhiteSpace(cnfPwd)) throw new Exception("Confirm Password is required and it cannot be null or empty.");
            //compare
            if (pwd.Trim().ToString() == cnfPwd.Trim().ToString())
            {
                return true;
            }
            else return false;
        }
        public bool ValidateOldPwdWithStoredEncodedPwd(string pwdPlainText, string cnfPwdEncoded)
        {
            if (!string.IsNullOrWhiteSpace(pwdPlainText) && !string.IsNullOrWhiteSpace(cnfPwdEncoded)) throw new Exception("Password and Confirm password are required and it cannot be null or empty.");
            else if (!string.IsNullOrWhiteSpace(pwdPlainText)) throw new Exception("Password is required and it cannot be null or empty.");
            else if (!string.IsNullOrWhiteSpace(cnfPwdEncoded)) throw new Exception("Confirm Password is required and it cannot be null or empty.");
            //compare
            if (pwdPlainText.Trim().ToString() == base64Decode2(cnfPwdEncoded.Trim().ToString()))
            {
                return true;
            }
            else return false;
        }
    }
}
