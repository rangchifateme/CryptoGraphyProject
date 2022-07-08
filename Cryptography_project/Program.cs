using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cryptography_project
{
    public class keys
    {
        public string keyValue { get; set; }
        public int count { get; set; }
    }
    class Program
    {
        static Random random = new Random();
        public static List<keys> Keys = new List<keys>();
        public static List<string> KeyResults = new List<string>();
        public static string GetRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }
        static void Main(string[] args)
        {
            string hexValues = "";
            string[] Subkeys = new string[5];
            string[] BinarySubkeys = new string[5];
            
            Subkeys[0] = "0B01";
            Subkeys[1] = "0A04";
            Subkeys[2] = "060B";
            Subkeys[3] = "0B0B";
            Subkeys[4] = "0507";
            Console.WriteLine("Please enter 5 keys in 16 bit hex format or for using default values just write default");
            hexValues = Console.ReadLine();
            for (int i = 0; i < 5; i++)
            {
                
                if (hexValues != "default")
                {
                    Subkeys[i] = hexValues;
                    Console.WriteLine("K["+ (i + 1).ToString() + "]=" + Subkeys[i]);
                   if(i<4) hexValues = Console.ReadLine();
                }
                
                else Console.WriteLine("K[" + (i+1).ToString() + "]=" + Subkeys[i]);
                BinarySubkeys[i] = String.Join(String.Empty, Subkeys[i].Select(
            c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
             ));

            }
            var PlainText = GetRandomHexNumber(4);
            string[] PlainText1 = new string[5000];
            string[] cipherText1 = new string[5000];
            
            string[] PlainText2 = new string[5000];
            string[] cipherText2 = new string[5000];
            for(int i = 0; i < 5000; i++)
            {
                PlainText1[i] = GetRandomHexNumber(4);
                PlainText2[i] = BinaryStringToHexString(XorOperation(HexToBinaryConvertor(PlainText1[i]), HexToBinaryConvertor("0B00"),16));
                string binarystringPlainText1 = String.Join(String.Empty, PlainText1[i].Select(
            c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
             ));
                string binarystringPlainText2 = String.Join(String.Empty, PlainText2[i].Select(
                c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                 ));
                cipherText1[i] = Encryption(binarystringPlainText1, BinarySubkeys);
                    cipherText2[i] = Encryption(binarystringPlainText2, BinarySubkeys);
                if (i < 10)
                {
                    Console.WriteLine("plainText1 [" + (i + 1).ToString() + "]is:" + binarystringPlainText1);
                    Console.WriteLine("plainText2 [" + (i + 1).ToString() + "]is:" + binarystringPlainText2);
                   
                    Console.WriteLine("cipherText1 [" + (i + 1).ToString() + "] result is: " + cipherText1[i]);
                    Console.WriteLine("cipherText2 [" + (i + 1).ToString() + "] result is: " + cipherText2[i]);
                }
            }
            for(int i =0; i < 5000; i++)
            {
                var ciphertextDifference = XorOperation(cipherText1[i], cipherText2[i],16);
                string[] ciphertextParts = new string[4];
                int k = 0;
                for (int p = 0; p < 16; p += 4)
                {
                    ciphertextParts[k] = ciphertextDifference.Substring(p, 4);
                    k++;
                }
                //if (ciphertextParts[0] == "0000" && ciphertextParts[2] == "0000")
                //{
                    if (i < 10) {
                        Console.WriteLine("C1: {0} , C2: {1}",cipherText1[i], cipherText2[i]);
                            }
                    var a = GuessDecryption(cipherText1[i], cipherText2[i]);
                //}

            }
            Console.WriteLine("IfCounter is: " + Ifcounter.ToString());
            //var sub = (Convert.ToInt32(hexc1, 16) - Convert.ToInt64(hexc2, 16)).ToString("X");
            //Console.WriteLine("ekhtelafe cipher:" + sub);
            //var dec = Decryption(cipherText1, BinarySubkeys);
            //var dec2 = Decryption(cipherText2, BinarySubkeys);
            //Console.WriteLine("Decryption result is: "+dec);
            var outcomeKeys = Keys.OrderByDescending(x => x.count);
  //          double prob = (double) outcomeKeys.FirstOrDefault().count / 5000;
            Console.WriteLine("key with the most repeat is:{0} with {1} count repeat", Keys.OrderByDescending(x => x.count).FirstOrDefault().keyValue, Keys.OrderByDescending(x => x.count).FirstOrDefault().count);
            Console.ReadLine();
           
        }
        //Encryption Operation
        public static string Encryption(string binarystringPlainText, string[] BinarySubkeys)
        {
            string cipherText = binarystringPlainText;
            for (int i = 0; i < 5; i++)
            {
                cipherText = XorOperation(cipherText, BinarySubkeys[i],16);
                if (i < 4)
                {
                    cipherText = Sbox(cipherText);
                    if (i < 3)
                        cipherText = Permutation(cipherText);
                }
            }
            return cipherText;
        }
        //Guess decryption
        public static int counter = 0;
        public static int Ifcounter = 0;
        public static bool GuessDecryption(string cipher1,string cipher2)
        {
            
            string plainText1 = cipher1;
            string plainText2 = cipher2;

            bool found = false;
            
            for(int i = 0; i < 16; i++)
            {
                
                for(int j = 0; j < 16; j++)
                {
                    var key = "0"+i.ToString("X")+ "0" + j.ToString("X") + "";
                    plainText1 = XorOperation(cipher1, HexToBinaryConvertor(key),16);
                    plainText1 = SboxReverse(cipher1, true);
                    var a = plainText1.Substring(8, 4);
                   
                    plainText2 = XorOperation(cipher2, HexToBinaryConvertor(key),16);
                    plainText2 = SboxReverse(cipher2, true);
                       
                            var aa = XorOperation(plainText1, plainText2, 16);
                    if(counter<10)
                    {
                        Console.WriteLine("PlainText1: {0}, PlainText2: {1}", plainText1, plainText2);
                        counter++;
                    }
                            if (XorOperation(plainText1, plainText2, 16) == "0000010100000101")
                            {
                                Ifcounter++;
                        
                                var existKey = Keys.FirstOrDefault(x => x.keyValue == key);
                                if (existKey != null)
                                {
                                    existKey.count++;
                                }
                                else
                                    Keys.Add(new keys { keyValue = key, count = 1 });
                                found = true;
                        if (Ifcounter < 100)
                        {
                            Console.WriteLine("Our keys["+(Ifcounter+1).ToString()+"] :"+ HexToBinaryConvertor(key));
                        }
                                //break;
                            }
                       
                }
                //if (found == true) break;
            }
            return found;
        }
        //Decryption Operation
        public static string Decryption(string cipherText, string[] BinarySubkeys)
        {
            string plainText = cipherText;
            for (int i = 4; i >= 0; i--)
            {
                plainText = XorOperation(plainText, BinarySubkeys[i],16);
                if (i > 0)
                {
                    if (i < 4)
                        plainText = Permutation(plainText);
                    plainText = SboxReverse(plainText);
                    
                }
            }
            return plainText;
        }
        //Permutation Operation
        public static string Permutation(string binary)
        {
            string permutationResult = "";
            for (int i = 1; i < 17; i++)
            {
                permutationResult += binary[PermutationOrder(i) - 1];
            }
            return permutationResult;
        }
        //Binary To Hex
        public static string BinaryStringToHexString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                return binary;

            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

            // TODO: check all 1's or 0's... throw otherwise

            int mod4Len = binary.Length % 8;
             if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }
        //SboxOperation
        public static string Sbox(string input)
        {
            input = BinaryStringToHexString(input);
            string SboxResult = "";
            foreach(char p in input)
            {
                SboxResult += SboxConvertor(p.ToString());
            }
            return String.Join(String.Empty, SboxResult.Select(
           c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
             ));
             
        }
        public static string SboxReverse(string input, bool just2And4= false)
        {
            input = BinaryStringToHexString(input);
            string SboxResult = "";
            var count = 0;
            foreach(char p in input)
            {
                if (just2And4 == true)
                {
                    if (count == 1 || count == 3)
                        SboxResult += SboxReversConvertor(p.ToString());
                    else SboxResult += "0";
                }
                else SboxResult += SboxReversConvertor(p.ToString());
                count++;
            }
            return String.Join(String.Empty, SboxResult.Select(
           c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
             ));
             
        }
        //XOR Operation
        public static string XorOperation(string input,string key,int bitNumber)
        {string xorRes = "";
            for (int i = 0; i< bitNumber; i++)
                {
                    if (input[i] == key[i])
                        xorRes += "0";
                    else xorRes += "1";
                }
            return xorRes;
        }
           
                
                
public static string HexToBinaryConvertor(string hex)
        {

            return String.Join(String.Empty, hex.Select(
            c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
             ));
        }
        public static string SboxConvertor(string hex)
        {
            switch (hex)
            {
                case "0": return "E";
                case "1": return "4";
                case "2": return "D";
                case "3": return "1";
                case "4": return "2";
                case "5": return "F";
                case "6": return "B";
                case "7": return "8";
                case "8": return "3";
                case "9": return "A";
                case "A": return "6";
                case "B": return "C";
                case "C": return "5";
                case "D": return "9";
                case "E": return "0";
                case "F": return "7";
                default: return "error";
            }
        }
        public static string SboxReversConvertor(string hex)
        {
            switch (hex)
            {
                case "0": return "E";
                case "1": return "3";
                case "2": return "4";
                case "3": return "8";
                case "4": return "1";
                case "5": return "C";
                case "6": return "A";
                case "7": return "F";
                case "8": return "7";
                case "9": return "D";
                case "A": return "9";
                case "B": return "6";
                case "C": return "B";
                case "D": return "2";
                case "E": return "0";
                case "F": return "5";
                default: return "error";
            }
        }
        public static int PermutationOrder(int i)
        {
            switch (i)
            {
                case 1: return 1;
                case 2: return 5;
                case 3: return 9;
                case 4: return 13;
                case 5: return 2;
                case 6: return 6;
                case 7: return 10;
                case 8: return 14;
                case 9: return 3;
                case 10: return 7;
                case 11: return 11;
                case 12: return 15;
                case 13: return 4;
                case 14: return 8;
                case 15: return 12;
                case 16: return 16;
                default: return 0;
            }
        }

    }
}
