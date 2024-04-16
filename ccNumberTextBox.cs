using System.ComponentModel;
namespace IDSBase
{
    public class ccNumberTextBox : TextBox
    {

        #region 変数定義
        private const Int32 WM_PASTE = 0x302;
        private Int32 intOldSelection;
        private Int32 intOldLength;
        private Boolean blnAllFlg = false;
        private Boolean blnKeyNumber = false;

        private Int32 _DecimalPlaces = 2;
        private Int32 _DigitInteger = 10;
        private Boolean _Minus = true;

        private Boolean IsNormalDel = false;
        private Boolean Is2ConmaDel = false;

        private Boolean IsDecDel = false;
        private Boolean IsDecBack = false;
        #endregion


        /// <summary>
        /// コンストラクタ
        /// </summary>
        #region  public ccNumberTextBox()
        public ccNumberTextBox()
        {
            
            base.AutoSize = false;
            base.DoubleBuffered = true;
            base.MinimumSize = new Size(0, 0);
            base.MaximumSize = new Size(0, 0);
            base.BorderStyle = BorderStyle.None;
            this.Margin = new Padding(0, 0, 0, 0);

        }
        #endregion


        //---------------------------------------------------
        //               Property(プロパティ）
        //---------------------------------------------------
        #region プロパティ

        [Category("表示")]
        [Description("整数桁")]
        [DefaultValue(10L)]
        public int DigitInteger
        {
            get { return this._DigitInteger; }
            set
            {
                if (this._DigitInteger != value)
                {
                    this._DigitInteger = value;
                    this.Invalidate();
                }
            }
        }

        [Category("表示")]
        [Description("小数点有効桁")]
        [DefaultValue(2L)]
        public int DecimalPlaces
        {
            get { return this._DecimalPlaces; }
            set
            {
                if (this._DecimalPlaces != value)
                {
                    this._DecimalPlaces = value;
                    this.Invalidate();
                }
            }
        }

        [Category("表示")]
        [Description("マイナスの有効・無効")]
        [DefaultValue(true)]
        public bool Minus
        {
            get { return this._Minus; }
            set
            {
                if (this._Minus != value)
                {
                    this._Minus = value;
                    this.Invalidate();
                }
            }
        }

        #endregion


        /// <summary>
        /// InitLayout
        /// </summary>
        #region protected override void InitLayout()
        protected override void InitLayout()
        {
            base.InitLayout();

            var ParentObj = this.Parent;

            this.TextAlign = HorizontalAlignment.Right;
            this.ImeMode = ImeMode.Off;
            this.Multiline = false;
        }
        #endregion


        //---------------------------------------------------
        //              Event（イベント）
        //---------------------------------------------------

        /// <summary>
        /// OnKeyDown イベント
        /// </summary>
        /// <param name="e"></param>
        #region  protected override void OnKeyDown(KeyEventArgs e)
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // 読み取り専用の場合、キー入力を抑制して終了
            if (this.ReadOnly)
            {
                e.SuppressKeyPress = true; 
                return;
            }
            if (this.SelectionLength > 1 && (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete))
            {
                Boolean period = false;
                Int32 selectionEnd = this.SelectionStart + this.SelectionLength;

                for (int i = this.SelectionStart; i < selectionEnd; i++)
                {
                    if (period)
                    {
                        //カーソル位置を保持
                        Int32 selection = this.SelectionStart;

                        //小数点の数値を０に置き換え
                        this.Text = this.Text.Remove(i, 1).Insert(i, "0");

                        //カーソル位置を戻す(Remove,Insert等を行うとSelectionStartの値が０に戻るため
                        this.SelectionStart = selection;

                        continue;
                    }
                   

                    if (this.Text.Substring(i, 1) == ".")
                    {
                        period = true;
                    }

                    //選択位置（カーソル）をピリオドに指定する
                    this.Select(this.SelectionStart, 0);

                    e.SuppressKeyPress = true;
                }
            }

            // テキストボックス内でのカーソル位置が有効な場合(カーソル位置がピリオドより大きく最大の長さより小さい）
            if (e.KeyCode == Keys.Back && this.SelectionStart > this.Text.IndexOf('.') && this.SelectionStart <= this.Text.Length && this.Text.IndexOf('.') != -1)
            {

                //選択範囲が小数全てだった時の処理
                if (e.KeyCode == Keys.Back && this.SelectionStart + this.SelectionLength == this.Text.Length && this.Text.Length - this.SelectionLength == this.Text.IndexOf('.') + 1)
                {
                    for (int i = this.SelectionStart; i < this.Text.Length; i++)
                    {
                      
                        //カーソル位置を保持
                        Int32 selection = this.SelectionStart;

                        //小数点の数値を０に置き換え
                        this.Text = this.Text.Remove(i, 1).Insert(i, "0");

                        //カーソル位置を戻す(Remove,Insert等を行うとSelectionStartの値が０に戻るため
                        this.SelectionStart = selection;

                    }

                    //選択位置（カーソル）をピリオドに指定する
                    this.Select(this.Text.IndexOf('.') + 1, 0);

                    e.SuppressKeyPress = true;
                }
                IsDecBack = true;
            }
            else
            {
                IsDecBack = false;
            }

            // テキストボックス内でのカーソル位置が有効な場合(カーソル位置がピリオドより大きく最大の長さより小さい）
            if (e.KeyCode == Keys.Delete && this.SelectionStart > this.Text.IndexOf('.') && this.SelectionStart <= this.Text.Length && this.Text.IndexOf('.') != -1)
            {
                IsDecDel = true;
            }
            else
            {
                IsDecDel = false;
            }

            //整数桁でDeleteが押された時で２つ右隣りがカンマだった時の処理
            if(e.KeyCode == Keys.Delete && this.SelectionStart < this.Text.Length -1 && this.Text.Substring(this.SelectionStart + 1,1) == "," && this.Text.IndexOf('.') != -1 && this.SelectionStart != 0)
            {
                Is2ConmaDel = true;
            }
            else
            {
                Is2ConmaDel = false;
            }

            if(e.KeyCode == Keys.Delete && this.SelectionStart < this.Text.IndexOf('.') && this.Text.Substring(this.SelectionStart + 1, 1) != "," && this.SelectionStart != 0)
            {
                IsNormalDel = true;
            }
            else
            {
                IsNormalDel = false;
            }
            //複数選択範囲が１つで選択がピリオドのみだった時
            if(this.SelectionLength == 1 && this.SelectionStart == this.Text.IndexOf('.'))
            {
                //カーソルをピリオドに合わせて範囲選択を解除して処理を行わない
                this.Select(this.Text.IndexOf('.'), 0);
                e.SuppressKeyPress = true;
                return;
            }
            
            // ピリオドまたは小数点キーが押された場合
            if (e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal)
            {
                // カーソル位置がピリオドの場合
                if (this.Text.Substring(this.SelectionStart, 1) == ".")
                {
                    // カーソルを1つ後ろに移動してキー入力を抑制
                    this.SelectionStart += 1;
                    e.SuppressKeyPress = true;
                }
                else
                {
                    // それ以外の場合、キー入力を抑制
                    e.SuppressKeyPress = true;
                }
            }
            // 入力されたキーに対する処理
            string strKeyValue = "";
            switch (e.KeyCode)
            {
                // 数字キー
                case Keys.D0:
                case Keys.NumPad0:
                    strKeyValue = "0";
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    strKeyValue = "1";
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    strKeyValue = "2";
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    strKeyValue = "3";
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    strKeyValue = "4";
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    strKeyValue = "5";
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    strKeyValue = "6";
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    strKeyValue = "7";
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    strKeyValue = "8";
                    break;
                case Keys.D9:
                case Keys.NumPad9:
                    strKeyValue = "9";
                    break;
                default:
                    strKeyValue = "";
                    break;
            }
            // 入力されたキーが数値の場合、フラグを立てる
            if (strKeyValue.CompareTo("0") >= 0 && strKeyValue.CompareTo("9") <= 0)
            {
                blnKeyNumber = true;
            }
            else
            {
                blnKeyNumber = false;
            }
            // 整数部の桁数を取得
            int intDigitInteger = 0;

            string strConvText = this.Text.Replace(",", "");

            // 負の値が許可されている場合、マイナス記号を除外
            if (_Minus)
            {
                strConvText = strConvText.Replace("-", "");
            }
            // 小数点以下の桁数が指定されており、テキストが存在する場合、小数点以下を削除
            if (_DecimalPlaces > 0 && this.Text.Length > 0)
            {
                strConvText = strConvText.Substring(0, strConvText.Length - _DecimalPlaces - 1);
            }
            // 整数部の桁数を取得
            if (!string.IsNullOrEmpty(strConvText))
            {
                intDigitInteger = strConvText.Length;
            }
            // 整数部の桁数が制限を超えている場合
            if (_DigitInteger <= intDigitInteger)
            {
                // 数字キーが押された場合
                if (blnKeyNumber)
                {
                    // カーソル位置がテキストの最後でない場合
                    if (this.SelectionStart >= 0 && this.SelectionStart < this.Text.Length)
                    {
                        string currentChar = this.Text.Substring(this.SelectionStart, 1);
                        // 現在の文字がピリオド、カンマ、マイナス記号でない場合、選択を1文字にする
                        if (currentChar != "." && currentChar != "," && currentChar != "-" && this.SelectionStart != this.Text.Length)
                        {
                            this.SelectionLength = 1;
                        }
                        else
                        {
                            e.SuppressKeyPress = true;
                        }
                    }
                   
                }
                else
                {
                    // 数字以外のキーが押された場合、キー入力を抑制
                    if (e.KeyCode != Keys.Delete &&
                        e.KeyCode != Keys.Back &&
                        e.KeyCode != Keys.Right &&
                        e.KeyCode != Keys.Left &&
                        e.KeyCode != Keys.Subtract &&
                        e.KeyCode != Keys.OemMinus)
                    {
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else
            {
                // 整数部の桁数が制限内にある場合、キー入力を制限
                if (((e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)) &&
                    e.KeyCode != Keys.NumPad0 &&
                    e.KeyCode != Keys.NumPad1 &&
                    e.KeyCode != Keys.NumPad2 &&
                    e.KeyCode != Keys.NumPad3 &&
                    e.KeyCode != Keys.NumPad4 &&
                    e.KeyCode != Keys.NumPad5 &&
                    e.KeyCode != Keys.NumPad6 &&
                    e.KeyCode != Keys.NumPad7 &&
                    e.KeyCode != Keys.NumPad8 &&
                    e.KeyCode != Keys.NumPad9 &&
                    e.KeyCode != Keys.Subtract &&
                    e.KeyCode != Keys.Delete &&
                    e.KeyCode != Keys.Back &&
                    e.KeyCode != Keys.Right &&
                    e.KeyCode != Keys.Left &&
                    e.KeyCode != Keys.Subtract &&
                    e.KeyCode != Keys.OemMinus &&
                    e.KeyCode != Keys.Decimal &&
                    e.KeyCode != Keys.OemPeriod)
                {
                    e.SuppressKeyPress = true;
                }
            }
            // 前回の選択位置とテキストの長さを保存
            intOldSelection = this.SelectionStart;
            intOldLength = this.TextLength;

            // カーソル位置が先頭で全選択されている場合のフラグを設定
            if (this.SelectionStart >= 0 && this.SelectionStart < this.Text.Length)
{
                if (this.SelectionStart != 0)
                {
                    if (e.KeyCode == Keys.Back && this.Text.Substring(this.SelectionStart - 1, 1) == ".")
                    {
                        e.SuppressKeyPress = true;
                    }
                    if (e.KeyCode == Keys.Delete && this.Text.Substring(this.SelectionStart, 1) == ".")
                    {
                        e.SuppressKeyPress = true;
                    }
                }
            }

            // マイナス記号キーが押された場合
            if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
            {
                // マイナス記号が許可されている場合、テキストの符号を反転させる
                if (_Minus) this.Text = TurnOver(this.Text);
                // キー入力を抑制
                e.SuppressKeyPress = true;
            }

            // テキストボックスが先頭で全選択されているかどうかを判定
            if (this.SelectionStart == 0 && this.SelectionLength == this.Text.Length)
            {
                // テキストボックスが先頭で全選択されている場合、フラグを立てる
                blnAllFlg = true;
            }
            else
            {
                // それ以外の場合、フラグをクリアする
                blnAllFlg = false;
            }
        }
        #endregion


        /// <summary>
        /// OnEnter イベント
        /// </summary>
        /// <param name="e"></param>
        #region protected override void OnEnter(EventArgs e)
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            // テキストボックス内の全てのテキストを選択状態にする
            this.SelectAll();
        }
        #endregion


        /// <summary>
        ///OnTextChanged イベント
        /// </summary>
        /// <param name="e"></param>
        #region  protected override void OnTextChanged(EventArgs e)
        protected override void OnTextChanged(EventArgs e)
        {
            // テキストを数値入力用に整形する
            this.Text = InputNumerical(this.Text);

            

            // 新しい選択位置を計算する
            int intNewSelection = 0;
            if (this.TextLength > intOldLength)
            {
                intNewSelection = intOldSelection + (this.Text.Length - intOldLength);
            }
            else
            {
                intNewSelection = intOldSelection - (intOldLength - this.Text.Length);
            }

            // 新しい選択位置を設定する
            if (intNewSelection >= 0)
            {
                switch (this.Text)
                {
                    case "0":
                        this.SelectionStart = this.Text.Length;
                        break;
                    case "0.00":
                        this.SelectionStart = 1;
                        break;
                    default:
                        this.SelectionStart = intNewSelection;
                        break;
                }
            }

            //小数点の位置でBackSpaceを押された時のフラグ処理
            if (IsDecBack)
            {
              this.SelectionStart -= 1;
            }

            //小数点の位置でDeleteを押された時のフラグ処理
            if(IsDecDel)
            {
                this.SelectionStart = this.Text.IndexOf('.') + 1;
            }
            //整数桁での２つとなりがカンマだった時の処理
            if(Is2ConmaDel)
            {
                this.SelectionStart += 2;
            }
            if(IsNormalDel)
            {
                this.SelectionStart += 1;
            }

            // 小数点以下の桁数が制限を超えている場合、選択位置を調整する
            if (this.SelectionStart >= (this.Text.Length - _DecimalPlaces) && _DecimalPlaces > 0)
            {
                if (blnKeyNumber) this.SelectionStart += 1;
            }

            // テキストボックスが先頭で全選択されている場合、選択位置を調整する
            if (blnAllFlg) this.SelectionStart = 1;
        }
        #endregion


        //----------------------------------------------------
        //                  Mesod(メソッド）
        //----------------------------------------------------


        /// <summary>
        /// 文字列を数値入力用に整形するメソッド
        /// </summary>
        /// <param name="old_str"></param>
        /// <returns></returns>
        #region public string InputNumerical(string old_str)
        public string InputNumerical(string old_str)
        {
            string new_str = old_str;
            if (old_str.Length > 0)
            {
                // カンマを除去した文字列を取得
                string pure_str = old_str.Replace(",", "");
                string strMinus = "";
                string pure_str2 = "";

                // 先頭がマイナス記号である場合、マイナス記号を除去して純粋な数値文字列を取得
                if (pure_str.Substring(0, 1) == "-")
                {
                    strMinus = "-";
                    pure_str2 = pure_str.Replace("-", "");
                }
                else
                {
                    pure_str2 = pure_str;
                }

                // 小数点以下の桁数を指定して数値をフォーマットするためのNumberFormatInfoオブジェクトを作成
                System.Globalization.NumberFormatInfo numberFormatInfo = new System.Globalization.NumberFormatInfo();
                numberFormatInfo.NumberDecimalDigits = _DecimalPlaces;

                try
                {
                    // 整数部と小数部を合わせて数値に変換
                    decimal number = Math.Round(Convert.ToDecimal(pure_str2),DecimalPlaces,MidpointRounding.ToZero);
                    // 数値を指定した桁数でフォーマットして新しい文字列を生成
                    new_str = number.ToString("N", numberFormatInfo);
                    // マイナス記号を付加して最終的な数値文字列を生成
                    new_str = strMinus + new_str;
                }
                catch
                {
                    // 変換に失敗した場合、元の文字列の最後の文字を除去してエラーを修正
                    new_str = old_str.Substring(0, old_str.Length - 1);
                }
            }
            return new_str;
        }
        #endregion


        /// <summary>
        /// 符号を反転するメソッド
        /// </summary>
        /// <param name="old_str"></param>
        /// <returns></returns>
        #region public string TurnOver(string old_str)
        public string TurnOver(string old_str)
        {
            string new_str = old_str;
            // 文字列が空でないかつ"0"でない場合
            if (old_str.Length > 0 && old_str != "0")
            {
                // 先頭がマイナス記号である場合
                if (old_str.Substring(0, 1) == "-")
                {
                    // マイナス記号を除去
                    new_str = old_str.Substring(1);
                }
                else
                {
                    // マイナス記号を付与
                    new_str = "-" + old_str;
                }
            }
            return new_str;
        }
        #endregion

        /// <summary>
        /// テキストボックスのウィンドウメッセージを処理するメソッド
        /// </summary>
        /// <param name="m"></param>
        #region protected override void WndProc(ref Message m)
        protected override void WndProc(ref Message m)
        {
            // ペーストされたデータをチェックする
            if (m.Msg == WM_PASTE)
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData != null && iData.GetDataPresent(DataFormats.Text))
                {
                    string clipStr = (string)iData.GetData(DataFormats.Text);
                    // ペーストされたデータがチェックをパスしない場合、処理を中断
                    if (!DataCheck(clipStr)) return;
                }
            }
            base.WndProc(ref m);
        }
        #endregion


        /// <summary>
        /// ペーストされたデータをチェックするメソッド
        /// </summary>
        /// <param name="ClipStr"></param>
        /// <returns></returns>
        #region private bool DataCheck(string ClipStr)
        private bool DataCheck(string ClipStr)
        {
            // 文字列が数値でない場合、チェックをパスしない
            if (!IsNumeric(ClipStr)) return false;
            // 数値が範囲外の場合、チェックをパスしない
            if (-9223372036854775807 > Convert.ToDouble(ClipStr) || 9223372036854775807 < Convert.ToDouble(ClipStr))
            {
                return false;
            }
            // 数値の桁数が制限を超えている場合、チェックをパスしない
            else if (Convert.ToString(Convert.ToInt64(ClipStr)).Length > _DigitInteger)
            {
                return false;
            }
            return true;
        }
        #endregion


        /// <summary>
        /// 文字列が数値かどうかを判定するメソッド
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        #region private bool IsNumeric(string text)
        private bool IsNumeric(string text)
        {
            double test;
            return double.TryParse(text, out test);
        }
        #endregion


    }
}
