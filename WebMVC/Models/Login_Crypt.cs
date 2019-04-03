using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMVC.Models
{
    public class Login_Crypt
    {
        /*
        public class SMS4
        {
            private static final int BLOCK = 16;
            private static final int DECRYPT = 0;
            private static final int ENCRYPT = 1;
            public static final int ROUND = 32;
            private int[] CK = new int[] { 462357, 472066609, 943670861, 1415275113, 1886879365, -1936483679, -1464879427, -993275175, -521670923, -66909679, 404694573, 876298825, 1347903077, 1819507329, -2003855715, -1532251463, -1060647211, -589042959, -117504499, 337322537, 808926789, 1280531041, 1752135293, -2071227751, -1599623499, -1128019247, -656414995, -184876535, 269950501, 741554753, 1213159005, 1684763257 };
            private byte[] Sbox = new byte[] { (byte)-42, (byte)-112, (byte)-23, (byte)-2, (byte)-52, (byte)-31, (byte)61, (byte)-73, (byte)22, (byte)-74, SecEngine.BCA_GET_CERT_DER_PUBLIC_KEY, (byte)-62, (byte)40, (byte)-5, (byte)44, (byte)5, (byte)43, (byte)103, (byte)-102, (byte)118, (byte)42, (byte)-66, (byte)4, (byte)-61, (byte)-86, (byte)68, SecEngine.BCA_GET_CERT_SUBJECT_EMAIL, (byte)38, (byte)73, (byte)-122, (byte)6, (byte)-103, (byte)-100, (byte)66, (byte)80, (byte)-12, (byte)-111, (byte)-17, (byte)-104, (byte)122, (byte)51, (byte)84, (byte)11, (byte)67, (byte)-19, (byte)-49, (byte)-84, (byte)98, (byte)-28, (byte)-77, (byte)28, (byte)-87, (byte)-55, (byte)8, (byte)-24, (byte)-107, Byte.MIN_VALUE, (byte)-33, (byte)-108, (byte)-6, (byte)117, (byte)-113, (byte)63, (byte)-90, (byte)71, (byte)7, (byte)-89, (byte)-4, (byte)-13, (byte)115, (byte)23, (byte)-70, (byte)-125, (byte)89, (byte)60, (byte)25, (byte)-26, (byte)-123, (byte)79, (byte)-88, (byte)104, (byte)107, (byte)-127, (byte)-78, (byte)113, (byte)100, (byte)-38, (byte)-117, (byte)-8, (byte)-21, SecEngine.BCA_GET_CERT_SUBJECT_PART, (byte)75, (byte)112, (byte)86, (byte)-99, (byte)53, (byte)30, (byte)36, (byte)14, (byte)94, (byte)99, (byte)88, (byte)-47, (byte)-94, (byte)37, (byte)34, (byte)124, (byte)59, (byte)1, (byte)33, (byte)120, (byte)-121, (byte)-44, (byte)0, (byte)70, (byte)87, (byte)-97, (byte)-45, (byte)39, (byte)82, (byte)76, (byte)54, (byte)2, (byte)-25, (byte)-96, (byte)-60, (byte)-56, (byte)-98, (byte)-22, (byte)-65, (byte)-118, (byte)-46, (byte)64, (byte)-57, (byte)56, (byte)-75, (byte)-93, (byte)-9, (byte)-14, (byte)-50, (byte)-7, (byte)97, (byte)21, (byte)-95, (byte)-32, (byte)-82, (byte)93, (byte)-92, (byte)-101, (byte)52, (byte)26, (byte)85, (byte)-83, (byte)-109, (byte)50, (byte)48, (byte)-11, (byte)-116, (byte)-79, (byte)-29, (byte)29, (byte)-10, (byte)-30, (byte)46, (byte)-126, (byte)102, (byte)-54, (byte)96, (byte)-64, (byte)41, (byte)35, (byte)-85, (byte)13, (byte)83, (byte)78, (byte)111, (byte)-43, (byte)-37, (byte)55, (byte)69, (byte)-34, (byte)-3, (byte)-114, (byte)47, (byte)3, (byte)-1, (byte)106, (byte)114, (byte)109, (byte)108, (byte)91, (byte)81, (byte)-115, (byte)27, (byte)-81, (byte)-110, (byte)-69, (byte)-35, (byte)-68, Byte.MAX_VALUE, SecEngine.BCA_GET_CERT_SUBJECT_NAME, (byte)-39, (byte)92, (byte)65, (byte)31, SecEngine.BCA_GET_CERT_SUBJECT_STATE, (byte)90, (byte)-40, (byte)10, (byte)-63, (byte)49, (byte)-120, (byte)-91, (byte)-51, (byte)123, (byte)-67, (byte)45, (byte)116, (byte)-48, SecEngine.BCA_GET_CERT_SUBJECT_CITY, (byte)-72, (byte)-27, (byte)-76, (byte)-80, (byte)-119, (byte)105, (byte)-105, (byte)74, (byte)12, (byte)-106, (byte)119, (byte)126, (byte)101, (byte)-71, (byte)-15, (byte)9, (byte)-59, (byte)110, (byte)-58, (byte)-124, (byte)24, (byte)-16, (byte)125, (byte)-20, (byte)58, (byte)-36, (byte)77, (byte)32, (byte)121, (byte)-18, (byte)95, (byte)62, (byte)-41, (byte)-53, (byte)57, (byte)72 };

            private int Rotl(int x, int y)
            {
                return (x << y) | (x >>> (32 - y));
            }

            private int ByteSub(int A)
            {
                return ((((this.Sbox[(A >>> 24) & 255] & 255) << 24) | ((this.Sbox[(A >>> 16) & 255] & 255) << 16)) | ((this.Sbox[(A >>> 8) & 255] & 255) << 8)) | (this.Sbox[A & 255] & 255);
            }

            private int L1(int B)
            {
                return (((Rotl(B, 2) ^ B) ^ Rotl(B, 10)) ^ Rotl(B, 18)) ^ Rotl(B, 24);
            }

            private int L2(int B)
            {
                return (Rotl(B, 13) ^ B) ^ Rotl(B, 23);
            }

            void SMS4Crypt(byte[] Input, byte[] Output, int[] rk)
            {
                int[] x = new int[4];
                int[] tmp = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    tmp[0] = Input[(i * 4) + 0] & 255;
                    tmp[1] = Input[(i * 4) + 1] & 255;
                    tmp[2] = Input[(i * 4) + 2] & 255;
                    tmp[3] = Input[(i * 4) + 3] & 255;
                    x[i] = (((tmp[0] << 24) | (tmp[1] << 16)) | (tmp[2] << 8)) | tmp[3];
                }
                for (int r = 0; r < 32; r += 4)
                {
                    x[0] = x[0] ^ L1(ByteSub(((x[1] ^ x[2]) ^ x[3]) ^ rk[r + 0]));
                    x[1] = x[1] ^ L1(ByteSub(((x[2] ^ x[3]) ^ x[0]) ^ rk[r + 1]));
                    x[2] = x[2] ^ L1(ByteSub(((x[3] ^ x[0]) ^ x[1]) ^ rk[r + 2]));
                    x[3] = x[3] ^ L1(ByteSub(((x[0] ^ x[1]) ^ x[2]) ^ rk[r + 3]));
                }
                for (int j = 0; j < 16; j += 4)
                {
                    Output[j] = (byte)((x[3 - (j / 4)] >>> 24) & 255);
                    Output[j + 1] = (byte)((x[3 - (j / 4)] >>> 16) & 255);
                    Output[j + 2] = (byte)((x[3 - (j / 4)] >>> 8) & 255);
                    Output[j + 3] = (byte)(x[3 - (j / 4)] & 255);
                }
            }

            private void SMS4KeyExt(byte[] Key, int[] rk, int CryptFlag)
            {
                int r;
                int[] x = new int[4];
                int[] tmp = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    tmp[0] = Key[(i * 4) + 0] & 255;
                    tmp[1] = Key[(i * 4) + 1] & 255;
                    tmp[2] = Key[(i * 4) + 2] & 255;
                    tmp[3] = Key[(i * 4) + 3] & 255;
                    x[i] = (((tmp[0] << 24) | (tmp[1] << 16)) | (tmp[2] << 8)) | tmp[3];
                }
                x[0] = x[0] ^ -1548633402;
                x[1] = x[1] ^ 1453994832;
                x[2] = x[2] ^ 1736282519;
                x[3] = x[3] ^ -1301273892;
                for (r = 0; r < 32; r += 4)
                {
                    int i2 = r + 0;
                    int L2 = x[0] ^ L2(ByteSub(((x[1] ^ x[2]) ^ x[3]) ^ this.CK[r + 0]));
                    x[0] = L2;
                    rk[i2] = L2;
                    i2 = r + 1;
                    L2 = x[1] ^ L2(ByteSub(((x[2] ^ x[3]) ^ x[0]) ^ this.CK[r + 1]));
                    x[1] = L2;
                    rk[i2] = L2;
                    i2 = r + 2;
                    L2 = x[2] ^ L2(ByteSub(((x[3] ^ x[0]) ^ x[1]) ^ this.CK[r + 2]));
                    x[2] = L2;
                    rk[i2] = L2;
                    i2 = r + 3;
                    L2 = x[3] ^ L2(ByteSub(((x[0] ^ x[1]) ^ x[2]) ^ this.CK[r + 3]));
                    x[3] = L2;
                    rk[i2] = L2;
                }
                if (CryptFlag == 0)
                {
                    for (r = 0; r < 16; r++)
                    {
                        int mid = rk[r];
                        rk[r] = rk[31 - r];
                        rk[31 - r] = mid;
                    }
                }
            }

            public int sms4(byte[] in, int inLen, byte[] key, byte[] out, int CryptFlag)
            {
                int point = 0;
                int[] round_key = new int[32];
                SMS4KeyExt(key, round_key, CryptFlag);
                byte[] input = new byte[16];
                byte[] output = new byte[16];
                while (inLen >= 16)
                {
                    SMS4Crypt(Arrays.copyOfRange(in, point, point + 16), output, round_key);
                    System.arraycopy(output, 0, out, point, 16);
                    inLen -= 16;
                    point += 16;
                }
                return 0;
            }

            public static byte[] encodeSMS4(String plaintext, byte[] key)
            {
                if (plaintext == null || plaintext.equals(""))
                {
                    return null;
                }
                for (int i = plaintext.getBytes().length % 16; i < 16; i++)
                {
                    plaintext = plaintext + '\u0000';
                }
                return encodeSMS4(plaintext.getBytes(), key);
            }

            public static byte[] encodeSMS4(byte[] plaintext, byte[] key)
            {
                byte[] ciphertext = new byte[plaintext.length];
                int plainLen = plaintext.length;
                for (int k = 0; k + 16 <= plainLen; k += 16)
                {
                    int i;
                    byte[] cellPlain = new byte[16];
                    for (i = 0; i < 16; i++)
                    {
                        cellPlain[i] = plaintext[k + i];
                    }
                    byte[] cellCipher = encode16(cellPlain, key);
                    for (i = 0; i < cellCipher.length; i++)
                    {
                        ciphertext[k + i] = cellCipher[i];
                    }
                }
                return ciphertext;
            }

            public static byte[] decodeSMS4(byte[] ciphertext, byte[] key)
            {
                byte[] plaintext = new byte[ciphertext.length];
                int cipherLen = ciphertext.length;
                for (int k = 0; k + 16 <= cipherLen; k += 16)
                {
                    int i;
                    byte[] cellCipher = new byte[16];
                    for (i = 0; i < 16; i++)
                    {
                        cellCipher[i] = ciphertext[k + i];
                    }
                    byte[] cellPlain = decode16(cellCipher, key);
                    for (i = 0; i < cellPlain.length; i++)
                    {
                        plaintext[k + i] = cellPlain[i];
                    }
                }
                return plaintext;
            }

            public static String decodeSMS4toString(byte[] ciphertext, byte[] key)
            {
                byte[] bArr = new byte[ciphertext.length];
                return new String(decodeSMS4(ciphertext, key));
            }

            private static byte[] encode16(byte[] plaintext, byte[] key)
            {
                byte[] cipher = new byte[16];
                new SMS4().sms4(plaintext, 16, key, cipher, 1);
                return cipher;
            }

            private static byte[] decode16(byte[] ciphertext, byte[] key)
            {
                byte[] plain = new byte[16];
                new SMS4().sms4(ciphertext, 16, key, plain, 0);
                return plain;
            }

            private static byte[] encode32(byte[] plaintext, byte[] key)
            {
                byte[] cipher = new byte[32];
                new SMS4().sms4(plaintext, 32, key, cipher, 1);
                return cipher;
            }

            private static byte[] decode32(byte[] ciphertext, byte[] key)
            {
                byte[] plain = new byte[32];
                new SMS4().sms4(ciphertext, 32, key, plain, 0);
                return plain;
            }
        }
        */
    }
}