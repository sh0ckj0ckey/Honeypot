using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyPasswords.Helpers
{
    /* 来自https://www.cnblogs.com/shikyoh/p/6270026.html
     * 微软PinYinConverter很强大，但在多音字面前，犯了传统的错误，按拼音字母排序。如【强】微软居然优先【jiang】而不是】【qiang】
       所以不能优选 PinYinConverter。
       Npinyin很人性，很不错的第三方库，在传统多音字前优先使用率较高的，但在生僻字面前有点无法转换。（GetInitials(strChinese)  有Bug  如【洺】无法识别，但GetPinyin可以正常转换。）
       总结：优先Npinyin  翻译失败的使用微软PinYinConverter。目测完美。*/
    public class PinyinHelper
    {
        private static Encoding gb2312 = Encoding.GetEncoding("GB2312");

        /// <summary>
        /// 汉字转全拼
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string ConvertToAllSpell(string strChinese)
        {
            try
            {
                if (strChinese.Length != 0)
                {
                    StringBuilder fullSpell = new StringBuilder();
                    for (int i = 0; i < strChinese.Length; i++)
                    {
                        var chr = strChinese[i];
                        fullSpell.Append(GetSpell(chr));
                    }
                    return fullSpell.ToString().ToUpper();
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 取第一个字的首字母
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static char GetFirstSpell(string strChinese)
        {
            try
            {
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
