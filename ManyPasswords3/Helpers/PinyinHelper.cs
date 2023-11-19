using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.International.Converters.PinYinConverter;

namespace ManyPasswords3.Helpers
{
    /* 来自https://www.cnblogs.com/shikyoh/p/6270026.html
     * 微软PinYinConverter很强大，但在多音字面前，犯了传统的错误，按拼音字母排序。如【强】微软居然优先【jiang】而不是】【qiang】
       所以不能优选 PinYinConverter。
       Npinyin很人性，很不错的第三方库，在传统多音字前优先使用率较高的，但在生僻字面前有点无法转换。（GetInitials(strChinese)  有Bug  如【洺】无法识别，但GetPinyin可以正常转换。）
       总结：优先Npinyin  翻译失败的使用微软PinYinConverter。目测完美。
    */
    public class PinyinHelper
    {
        private static Encoding gb2312 = Encoding.GetEncoding("GB2312");

        /// <summary>
        /// 取第一个字的首字母
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static char GetFirstSpell(string strChinese)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strChinese))
                {
                    return '#';
                }

                strChinese = strChinese.Trim();

                // 数字
                if (Regex.IsMatch(strChinese, "^[0-9]"))
                {
                    return '#';
                }

                // 英文
                if (Regex.IsMatch(strChinese.Trim(), "^[a-zA-Z]"))
                {
                    return strChinese.ToUpper()[0];
                }

                // 特殊
                switch (strChinese[0])
                {
                    case 'ā':
                    case 'á':
                    case 'ǎ':
                    case 'à':
                        return 'A';
                    case 'ō':
                    case 'ó':
                    case 'ǒ':
                    case 'ò':
                        return 'O';
                    case 'ē':
                    case 'é':
                    case 'ě':
                    case 'è':
                    case 'ê':
                        return 'E';
                    case 'ī':
                    case 'í':
                    case 'ǐ':
                    case 'ì':
                        return 'I';
                    case 'ū':
                    case 'ú':
                    case 'ǔ':
                    case 'ù':
                        return 'U';
                    case 'ǖ':
                    case 'ǘ':
                    case 'ǚ':
                    case 'ǜ':
                    case 'ü':
                        return 'V';
                }

                if (strChinese.Length > 0)
                {
                    var firstSpell = GetSpell(strChinese[0])[0];
                    return firstSpell;
                }
            }
            catch { }
            return strChinese.Length > 0 ? strChinese[0] : '#';
        }

        private static string GetSpell(char chr)
        {
            try
            {
                var pinyin = NPinyin.Pinyin.GetPinyin(chr);
                // 如果用NPinyin已经转换成功，则converchr会是拼音，否则是原文字
                // 当converchr仍是文字的时候，ChineseChar.IsValidChar为true，则继续使用PinYinConverter转换
                bool isStillChinese = ChineseChar.IsValidChar(pinyin[0]);
                if (isStillChinese)
                {
                    ChineseChar chineseChar = new ChineseChar(chr);
                    foreach (string value in chineseChar.Pinyins)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            // chineseChar.Pinyins多音字可能有多个读音，返回第一个不为空的读音
                            // PinYinConverter得到的拼音结尾会有一个数字表示音调
                            return value.Remove(value.Length - 1, 1).ToUpper();
                        }
                    }
                }
                return pinyin.ToUpper();
            }
            catch { }
            return chr.ToString();
        }
    }
}
