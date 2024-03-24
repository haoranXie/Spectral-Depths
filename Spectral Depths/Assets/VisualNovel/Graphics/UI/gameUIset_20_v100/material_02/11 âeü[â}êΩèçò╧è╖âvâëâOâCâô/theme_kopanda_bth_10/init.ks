;--------------------------------------------------------------------------------
; ティラノスクリプト テーマプラグイン theme_kopanda_bth_10
; 作者:こ・ぱんだ
; https://kopacurve.blog.fc2.com/
;--------------------------------------------------------------------------------

[iscript]

mp.font_color    = mp.font_color    || "0xE8E8F2";
mp.name_color    = mp.name_color    || "0x61C2F2";
mp.frame_opacity = mp.frame_opacity || "255";
mp.font_color2   = mp.font_color2   || "0xE8E8F2";
mp.glyph         = mp.glyph         || "off";

if(TG.config.alreadyReadTextColor != "default") {
	TG.config.alreadyReadTextColor = mp.font_color2;
}

[endscript]

; 名前部分のメッセージレイヤ削除
[free name="chara_name_area" layer="message0"]

; メッセージウィンドウの設定
[position layer="message0" width="1280" height="204" top="516" left="0"]
[position layer="message0" frame="../others/plugin/theme_kopanda_bth_10/image/frame_message.png" margint="60" marginl="140" marginr="140" opacity="&mp.frame_opacity" page="fore"]

; 名前枠の設定
[ptext name="chara_name_area" layer="message0" color="&mp.name_color" size="26" x="80" y="521" width="400" align="center"]
[chara_config ptext="chara_name_area"]

; デフォルトのフォントカラー指定
[font color="&mp.font_color"]
[deffont color="&mp.font_color"]

; デフォルトのフォントサイズ指定
[font size="26"]
[deffont size="26"]

; クリック待ちグリフの設定（on設定時のみ有効）
[if exp="mp.glyph == 'on'"]
[glyph line="../../../data/others/plugin/theme_kopanda_bth_10/image/system/nextpage.png"]
[endif]

;=================================================================================

; 機能ボタンを表示するマクロ

;=================================================================================

; 機能ボタンを表示したいシーンで[add_theme_button]と記述してください（消去は[clearfix name="role_button"]で）
[macro name="add_theme_button"]

; デフォルトのメニューボタンを消す
[hidemenubutton]

; Q.Save
[button hint="簡易セーブ" name="role_button" role="quicksave" graphic="../others/plugin/theme_kopanda_bth_10/image/button/qsave.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/qsave2.png" x="104" y="690"]

; Q.Load
[button hint="簡易ロード" name="role_button" role="quickload" graphic="../others/plugin/theme_kopanda_bth_10/image/button/qload.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/qload2.png" x="216" y="690"]

;	Save
[button hint="セーブ" name="role_button" role="save" graphic="../others/plugin/theme_kopanda_bth_10/image/button/save.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/save2.png" x="332" y="690"]

;	Load
[button hint="ロード" name="role_button" role="load" graphic="../others/plugin/theme_kopanda_bth_10/image/button/load.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/load2.png" x="422" y="690"]

; Auto
[button hint="テキストの自動送り" name="role_button" role="auto" graphic="../others/plugin/theme_kopanda_bth_10/image/button/auto.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/auto2.png" x="516" y="690"]

; Skip
[button hint="テキストの早送り" name="role_button" role="skip" graphic="../others/plugin/theme_kopanda_bth_10/image/button/skip.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/skip2.png" x="606" y="690"]

; Backlog
[button hint="履歴" name="role_button" role="backlog" graphic="../others/plugin/theme_kopanda_bth_10/image/button/log.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/log2.png" x="690" y="690"]

; Screen
[button hint="画面サイズ切替" name="role_button" role="fullscreen" graphic="../others/plugin/theme_kopanda_bth_10/image/button/screen.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/screen2.png" x="806" y="690"]

;	Config
[button hint="環境設定" name="role_button" role="sleepgame" graphic="../others/plugin/theme_kopanda_bth_10/image/button/sleep.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/sleep2.png" storage="../others/plugin/theme_kopanda_bth_10/config.ks" x="910" y="690"]

; Menu
[button hint="メニュー" name="role_button" role="menu" graphic="../others/plugin/theme_kopanda_bth_10/image/button/menu.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/menu2.png" x="1018" y="690"]

; Title
[button hint="タイトル画面へ戻る" name="role_button" role="title" graphic="../others/plugin/theme_kopanda_bth_10/image/button/title.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/title2.png" x="1114" y="690"]

; Close
[button hint="テキスト非表示" name="role_button" role="window" graphic="../others/plugin/theme_kopanda_bth_10/image/button/close.png" enterimg="../others/plugin/theme_kopanda_bth_10/image/button/close2.png" x="1228" y="552"]

[endmacro]


;=================================================================================

; システムで利用するHTML,CSSの設定

;=================================================================================
; セーブ画面
[sysview type="save" storage="./data/others/plugin/theme_kopanda_bth_10/html/save.html"]

; ロード画面
[sysview type="load" storage="./data/others/plugin/theme_kopanda_bth_10/html/load.html"]

; バックログ画面
[sysview type="backlog" storage="./data/others/plugin/theme_kopanda_bth_10/html/backlog.html"]

; メニュー画面
[sysview type="menu" storage="./data/others/plugin/theme_kopanda_bth_10/html/menu.html"]

; CSS
[loadcss file="./data/others/plugin/theme_kopanda_bth_10/bth10.css"]

; メニュー画面からコンフィグを呼び出すための設定ファイル
[loadjs storage="plugin/theme_kopanda_bth_10/setting.js"]

;=================================================================================

; テストメッセージ出力プラグインの読み込み

;=================================================================================
[loadjs storage="plugin/theme_kopanda_bth_10/testMessagePlus/gMessageTester.js"]
[loadcss file="./data/others/plugin/theme_kopanda_bth_10/testMessagePlus/style.css"]

[macro name="test_message_start"]
[eval exp="gMessageTester.create()"]
[endmacro]

[macro name="test_message_end"]
[eval exp="gMessageTester.destroy()"]
[endmacro]

[macro name="test_message_reset"]
[eval exp="gMessageTester.currentTextNumber=0;gMessageTester.next(true)"]
[endmacro]


[return]
