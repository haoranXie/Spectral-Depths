; 2023/09/14 @ko10panda edit

;===============================================================================

; コンフィグ画面作成

;===============================================================================

[layopt layer="message0" visible="false"]
[clearfix]
[stop_keyconfig]
[free_layermode time="100" wait="true"]
[reset_camera time="100" wait="true"]
[hidemenubutton]

[iscript]

$(".layer_camera").empty();
$("#bgmovie").remove();

TG.config.autoRecordLabel = "true";

/*
 * コンフィグ内のみで使用する変数類
 * tf.img_path				画像類のパス
 * tf.btn_path_off		ボタン画像のファイル名（透明なので見えません）
 * tf.btn_path_on			ボタン画像のファイル名
 * tf.config_x				ボタンを配置するX座標
 * tf.config_y				ボタンを配置するY座標
 * tf.img_width				ボタンの幅
 * tf.img_height			ボタンの高さ
 *
 * tf.config_num_bgm		BGM音量のインデックス
 * tf.config_num_se			SE音量のインデックス
 * tf.config_num_ch			テキスト速度のインデックス
 * tf.config_num_auto		オートウェイトのインデックス
 *
 * tf.current_bgm_vol		  現在のBGM音量
 * tf.current_se_vol		  現在のSE音量
 * tf.current_ch_speed		現在のテキスト速度
 * tf.current_auto_speed	現在のオートウェイト
 * tf.text_skip				    現在の未読スキップの状態
 * tf.screen_size			    現在の画面サイズ
 *
 * f.prev_vol_list			BGMとSEの音量とインデックスを保存している配列（ミュート解除時に必要なため）
*/

tf.img_path 		      = '../others/plugin/theme_kopanda_bth_10/image/config/';
tf.btn_path_off 	    = tf.img_path + 'c_btn.gif';
tf.btn_path_on  	    = tf.img_path + 'gauge_active.png';
tf.btn_path_on_h      = tf.img_path + 'gauge_hover.png';
tf.config_x	 		      = [1111, 350, 422, 494, 566, 638, 710, 782, 854, 926, 998];
tf.config_y 		      = [199, 279, 359, 439];
tf.img_width 		      = 54;
tf.img_height 	      = 54;
tf.config_num_bgm;
tf.config_num_se;
tf.config_num_ch;
tf.config_num_auto;
tf.current_bgm_vol    = parseInt(TG.config.defaultBgmVolume);
tf.current_se_vol     = parseInt(TG.config.defaultSeVolume);
tf.current_ch_speed   = parseInt(TG.config.chSpeed);
tf.current_auto_speed = parseInt(TG.config.autoSpeed);

tf.text_skip ="ON";
	if(TG.config.unReadTextSkip != "true") {
		tf.text_skip ="OFF";
	}

tf.screen_size = (function() {

	if ((document.FullscreenElement !== undefined && document.FullscreenElement !== null) ||
    	(document.webkitFullscreenElement !== undefined && document.webkitFullscreenElement !== null) ||
      	(document.msFullscreenElement !== undefined && document.msFullscreenElement !== null)) {

    	return 'full';

 	} else {

  		return 'window';

	}
})();

switch(tf.current_bgm_vol) {
	case   0: tf.config_num_bgm =  0; break;
	case  10: tf.config_num_bgm =  1; break;
	case  20: tf.config_num_bgm =  2; break;
	case  30: tf.config_num_bgm =  3; break;
	case  40: tf.config_num_bgm =  4; break;
	case  50: tf.config_num_bgm =  5; break;
	case  60: tf.config_num_bgm =  6; break;
	case  70: tf.config_num_bgm =  7; break;
	case  80: tf.config_num_bgm =  8; break;
	case  90: tf.config_num_bgm =  9; break;
	case 100: tf.config_num_bgm = 10; break;

	default: break;
};

switch(tf.current_se_vol) {
	case   0: tf.config_num_se =  0; break;
	case  10: tf.config_num_se =  1; break;
	case  20: tf.config_num_se =  2; break;
	case  30: tf.config_num_se =  3; break;
	case  40: tf.config_num_se =  4; break;
	case  50: tf.config_num_se =  5; break;
	case  60: tf.config_num_se =  6; break;
	case  70: tf.config_num_se =  7; break;
	case  80: tf.config_num_se =  8; break;
	case  90: tf.config_num_se =  9; break;
	case 100: tf.config_num_se = 10; break;

	default: break;
};

switch(tf.current_ch_speed) {
	case 100: tf.config_num_ch =  0; break;
	case  80: tf.config_num_ch =  1; break;
	case  50: tf.config_num_ch =  2; break;
	case  40: tf.config_num_ch =  3; break;
	case  30: tf.config_num_ch =  4; break;
	case  25: tf.config_num_ch =  5; break;
	case  20: tf.config_num_ch =  6; break;
	case  11: tf.config_num_ch =  7; break;
	case   8: tf.config_num_ch =  8; break;
	case   5: tf.config_num_ch =  9; break;

	default: break;
};

switch(tf.current_auto_speed) {
	case 5000: tf.config_num_auto =  0; break;
	case 4500: tf.config_num_auto =  1; break;
	case 4000: tf.config_num_auto =  2; break;
	case 3500: tf.config_num_auto =  3; break;
	case 3000: tf.config_num_auto =  4; break;
	case 2500: tf.config_num_auto =  5; break;
	case 2000: tf.config_num_auto =  6; break;
	case 1300: tf.config_num_auto =  7; break;
	case  800: tf.config_num_auto =  8; break;
	case  500: tf.config_num_auto =  9; break;

	default: break;
};

// ミュート解除用
if(typeof f.prev_vol_list === 'undefined') {
	f.prev_vol_list = [tf.current_bgm_vol, tf.config_num_bgm, tf.current_se_vol, tf.config_num_se];
}

[endscript]

[cm]

[bg storage="&tf.img_path +'config_bg.png'" time="200"]
[button hint="ゲームに戻る" fix="true" graphic="&tf.img_path + 'back.png'" enterimg="&tf.img_path + 'back2.png'" target="*backtitle" x="1192" y="24"]

[jump target="*config_page"]

*config_page
[clearstack]
;-------------------------------------------------------------------------------
; BGM音量
;-------------------------------------------------------------------------------
[button name="bgmvol,bgmvol_10"  fix="true" target="*vol_bgm_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[1]"  y="&tf.config_y[0]" exp="tf.current_bgm_vol =  10; tf.config_num_bgm =  1"]
[button name="bgmvol,bgmvol_20"  fix="true" target="*vol_bgm_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[2]"  y="&tf.config_y[0]" exp="tf.current_bgm_vol =  20; tf.config_num_bgm =  2"]
[button name="bgmvol,bgmvol_30"  fix="true" target="*vol_bgm_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[3]"  y="&tf.config_y[0]" exp="tf.current_bgm_vol =  30; tf.config_num_bgm =  3"]
[button name="bgmvol,bgmvol_40"  fix="true" target="*vol_bgm_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[4]"  y="&tf.config_y[0]" exp="tf.current_bgm_vol =  40; tf.config_num_bgm =  4"]
[button name="bgmvol,bgmvol_50"  fix="true" target="*vol_bgm_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[5]"  y="&tf.config_y[0]" exp="tf.current_bgm_vol =  50; tf.config_num_bgm =  5"]
[button name="bgmvol,bgmvol_60"  fix="true" target="*vol_bgm_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[6]"  y="&tf.config_y[0]" exp="tf.current_bgm_vol =  60; tf.config_num_bgm =  6"]
[button name="bgmvol,bgmvol_70"  fix="true" target="*vol_bgm_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[7]"  y="&tf.config_y[0]" exp="tf.current_bgm_vol =  70; tf.config_num_bgm =  7"]
[button name="bgmvol,bgmvol_80"  fix="true" target="*vol_bgm_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[8]"  y="&tf.config_y[0]" exp="tf.current_bgm_vol =  80; tf.config_num_bgm =  8"]
[button name="bgmvol,bgmvol_90"  fix="true" target="*vol_bgm_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[9]"  y="&tf.config_y[0]" exp="tf.current_bgm_vol =  90; tf.config_num_bgm =  9"]
[button name="bgmvol,bgmvol_100" fix="true" target="*vol_bgm_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[10]" y="&tf.config_y[0]" exp="tf.current_bgm_vol = 100; tf.config_num_bgm = 10"]

; BGMミュート
[button hint="BGMミュート" name="bgmvol,bgmvol_0"   fix="true" target="*vol_bgm_mute" graphic="&tf.btn_path_off" width="24" height="24" x="&tf.config_x[0]" y="224"]

;-------------------------------------------------------------------------------
; SE音量
;-------------------------------------------------------------------------------
[button name="sevol,sevol_10"  fix="true" target="*vol_se_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[1]"  y="&tf.config_y[1]" exp="tf.current_se_vol =  10; tf.config_num_se =  1"]
[button name="sevol,sevol_20"  fix="true" target="*vol_se_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[2]"  y="&tf.config_y[1]" exp="tf.current_se_vol =  20; tf.config_num_se =  2"]
[button name="sevol,sevol_30"  fix="true" target="*vol_se_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[3]"  y="&tf.config_y[1]" exp="tf.current_se_vol =  30; tf.config_num_se =  3"]
[button name="sevol,sevol_40"  fix="true" target="*vol_se_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[4]"  y="&tf.config_y[1]" exp="tf.current_se_vol =  40; tf.config_num_se =  4"]
[button name="sevol,sevol_50"  fix="true" target="*vol_se_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[5]"  y="&tf.config_y[1]" exp="tf.current_se_vol =  50; tf.config_num_se =  5"]
[button name="sevol,sevol_60"  fix="true" target="*vol_se_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[6]"  y="&tf.config_y[1]" exp="tf.current_se_vol =  60; tf.config_num_se =  6"]
[button name="sevol,sevol_70"  fix="true" target="*vol_se_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[7]"  y="&tf.config_y[1]" exp="tf.current_se_vol =  70; tf.config_num_se =  7"]
[button name="sevol,sevol_80"  fix="true" target="*vol_se_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[8]"  y="&tf.config_y[1]" exp="tf.current_se_vol =  80; tf.config_num_se =  8"]
[button name="sevol,sevol_90"  fix="true" target="*vol_se_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[9]"  y="&tf.config_y[1]" exp="tf.current_se_vol =  90; tf.config_num_se =  9"]
[button name="sevol,sevol_100" fix="true" target="*vol_se_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[10]" y="&tf.config_y[1]" exp="tf.current_se_vol = 100; tf.config_num_se = 10"]

; SEミュート
[button hint="効果音ミュート" name="sevol,sevol_0"   fix="true" target="*vol_se_mute" graphic="&tf.btn_path_off" width="24" height="24" x="&tf.config_x[0]" y="304"]

;-------------------------------------------------------------------------------
; テキスト速度
;-------------------------------------------------------------------------------
[button name="ch,ch_100" fix="true" target="*ch_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[1]"  y="&tf.config_y[2]" exp="tf.set_ch_speed =100; tf.config_num_ch = 0"]
[button name="ch,ch_80"  fix="true" target="*ch_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[2]"  y="&tf.config_y[2]" exp="tf.set_ch_speed = 80; tf.config_num_ch = 1"]
[button name="ch,ch_50"  fix="true" target="*ch_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[3]"  y="&tf.config_y[2]" exp="tf.set_ch_speed = 50; tf.config_num_ch = 2"]
[button name="ch,ch_40"  fix="true" target="*ch_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[4]"  y="&tf.config_y[2]" exp="tf.set_ch_speed = 40; tf.config_num_ch = 3"]
[button name="ch,ch_30"  fix="true" target="*ch_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[5]"  y="&tf.config_y[2]" exp="tf.set_ch_speed = 30; tf.config_num_ch = 4"]
[button name="ch,ch_25"  fix="true" target="*ch_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[6]"  y="&tf.config_y[2]" exp="tf.set_ch_speed = 25; tf.config_num_ch = 5"]
[button name="ch,ch_20"  fix="true" target="*ch_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[7]"  y="&tf.config_y[2]" exp="tf.set_ch_speed = 20; tf.config_num_ch = 6"]
[button name="ch,ch_11"  fix="true" target="*ch_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[8]"  y="&tf.config_y[2]" exp="tf.set_ch_speed = 11; tf.config_num_ch = 7"]
[button name="ch,ch_8"   fix="true" target="*ch_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[9]"  y="&tf.config_y[2]" exp="tf.set_ch_speed =  8; tf.config_num_ch = 8"]
[button name="ch,ch_5"   fix="true" target="*ch_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[10]" y="&tf.config_y[2]" exp="tf.set_ch_speed =  5; tf.config_num_ch = 9"]

;-------------------------------------------------------------------------------
; オートウェイト
;-------------------------------------------------------------------------------
[button name="auto,auto_5000" fix="true" target="*auto_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[1]"  y="&tf.config_y[3]" exp="tf.set_auto_speed = 5000; tf.config_num_auto = 0"]
[button name="auto,auto_4500" fix="true" target="*auto_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[2]"  y="&tf.config_y[3]" exp="tf.set_auto_speed = 4500; tf.config_num_auto = 1"]
[button name="auto,auto_4000" fix="true" target="*auto_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[3]"  y="&tf.config_y[3]" exp="tf.set_auto_speed = 4000; tf.config_num_auto = 2"]
[button name="auto,auto_3500" fix="true" target="*auto_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[4]"  y="&tf.config_y[3]" exp="tf.set_auto_speed = 3500; tf.config_num_auto = 3"]
[button name="auto,auto_3000" fix="true" target="*auto_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[5]"  y="&tf.config_y[3]" exp="tf.set_auto_speed = 3000; tf.config_num_auto = 4"]
[button name="auto,auto_2500" fix="true" target="*auto_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[6]"  y="&tf.config_y[3]" exp="tf.set_auto_speed = 2500; tf.config_num_auto = 5"]
[button name="auto,auto_2000" fix="true" target="*auto_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[7]"  y="&tf.config_y[3]" exp="tf.set_auto_speed = 2000; tf.config_num_auto = 6"]
[button name="auto,auto_1300" fix="true" target="*auto_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[8]"  y="&tf.config_y[3]" exp="tf.set_auto_speed = 1300; tf.config_num_auto = 7"]
[button name="auto,auto_800"  fix="true" target="*auto_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[9]"  y="&tf.config_y[3]" exp="tf.set_auto_speed =  800; tf.config_num_auto = 8"]
[button name="auto,auto_500"  fix="true" target="*auto_speed_change" graphic="&tf.btn_path_off" enterimg="&tf.btn_path_on_h" width="&tf.img_width" height="&tf.img_height" x="&tf.config_x[10]" y="&tf.config_y[3]" exp="tf.set_auto_speed =  500; tf.config_num_auto = 9"]

;-------------------------------------------------------------------------------
; 未読スキップ
;-------------------------------------------------------------------------------
; 未読スキップ-ON
[button hint="全文スキップ" name="unread_on"  fix="true" target="*skip_on"  graphic="&tf.btn_path_off" width="68" height="24" x="351" y="530"]

; 未読スキップ-OFF
[button hint="既読のみスキップ" name="unread_off" fix="true" target="*skip_off" graphic="&tf.btn_path_off" width="74" height="24" x="462" y="530"]

;-------------------------------------------------------------------------------
; 画面サイズ
;-------------------------------------------------------------------------------
; フルスクリーン
[button hint="フルスクリーン" name="screen_full" fix="true" target="*screen_full" graphic="&tf.btn_path_off" width="70" height="24" x="785" y="530"]

; ウィンドウサイズ
[button hint="ウィンドウサイズ" name="screen_window" fix="true" target="*screen_window" graphic="&tf.btn_path_off" width="108" height="24" x="907" y="530"]

;-------------------------------------------------------------------------------
; コンフィグ起動時に読み込み
;-------------------------------------------------------------------------------
[layopt layer="0" visible="true"]

[call target="*load_bgm_img"]
[call target="*load_se_img"]
[call target="*load_ch_img"]
[call target="*load_auto_img"]
[call target="*load_skip_img"]
[call target="*load_screen_img"]

[test_message_start]

[s]

;-------------------------------------------------------------------------------
; コンフィグモード終了
;-------------------------------------------------------------------------------
*backtitle
[cm]
[layopt layer="message1" visible="false"]
[clearfix]
[start_keyconfig]
[clearstack]

[awakegame]

;===============================================================================

; ボタンクリック時の処理

;===============================================================================
;-------------------------------------------------------------------------------
; BGM音量
;-------------------------------------------------------------------------------
*vol_bgm_mute
[iscript]

if(tf.current_bgm_vol != 0) {
	f.prev_vol_list[0] = tf.current_bgm_vol;
	f.prev_vol_list[1] = tf.config_num_bgm;
	tf.current_bgm_vol = 0;
	tf.config_num_bgm  = 0;
} else {
	tf.current_bgm_vol = f.prev_vol_list[0];
	tf.config_num_bgm  = f.prev_vol_list[1];
}

[endscript]

*vol_bgm_change
[free layer="0" name="bgmvol" time="0" wait="true"]
[call target="*load_bgm_img"]
[bgmopt volume="&tf.current_bgm_vol"]

[return]

;-------------------------------------------------------------------------------
; SE音量
;-------------------------------------------------------------------------------
*vol_se_mute
[iscript]

if( tf.current_se_vol != 0 ) {
	f.prev_vol_list[2] = tf.current_se_vol;
	f.prev_vol_list[3] = tf.config_num_se;
	tf.current_se_vol = 0;
	tf.config_num_se  = 0;
} else {
	tf.current_se_vol = f.prev_vol_list[2];
	tf.config_num_se  = f.prev_vol_list[3];
}

[endscript]

*vol_se_change
[free layer="0" name="sevol" time="0" wait="true"]
[call target="*load_se_img"]
[seopt volume="&tf.current_se_vol"]

[return]

;-------------------------------------------------------------------------------
; テキスト速度
;-------------------------------------------------------------------------------
*ch_speed_change
[eval exp="tf.current_ch_speed = tf.set_ch_speed"]
[free layer="0" name="ch" time="0" wait="true"]
[call target="*load_ch_img"]
[configdelay speed="&tf.set_ch_speed"]
[test_message_reset]

[return]

;-------------------------------------------------------------------------------
; オート時のウェイト
;-------------------------------------------------------------------------------
*auto_speed_change
[eval exp="tf.current_auto_speed = tf.set_auto_speed"]
[free layer="0" name="auto" time="0" wait="true"]
[call target="*load_auto_img"]
[autoconfig speed="&tf.set_auto_speed"]

[return]

;-------------------------------------------------------------------------------
; スキップ処理-ON
;-------------------------------------------------------------------------------
*skip_on
[free layer="0" name="unread_off" time="10"]
[image layer="0" name="unread_on" storage="&tf.img_path + 'skipon_active.png'" x="351" y="530"]
[config_record_label skip="true"]

[return]

;-------------------------------------------------------------------------------
; スキップ処理-OFF
;-------------------------------------------------------------------------------
*skip_off
[free layer="0" name="unread_on" time="10"]
[image layer="0" name="unread_off" storage="&tf.img_path + 'skipoff_active.png'" x="462" y="530"]
[config_record_label skip="false"]

[return]

;-------------------------------------------------------------------------------
; 画面サイズ-フルスクリーン
;-------------------------------------------------------------------------------
*screen_full
[if exp="tf.screen_size == 'window'"]
	[screen_full]
	[free layer="0" name="screen_window" time="10"]
	[image layer="0" name="screen_full" storage="&tf.img_path + 'full_active.png'" x="785" y="530"]
	[eval exp="tf.screen_size = 'full'"]
[endif]

[return]

;-------------------------------------------------------------------------------
; 画面サイズ-ウィンドウサイズ
;-------------------------------------------------------------------------------
*screen_window
[if exp="tf.screen_size == 'full'"]
	[screen_full]
	[free layer="0" name="screen_full" time="10"]
	[image layer="0" name="screen_window" storage="&tf.img_path + 'window_active.png'" x="907" y="530"]
	[eval exp="tf.screen_size = 'window'"]
[endif]

[return]

;===============================================================================

; 画像更新

;===============================================================================
*load_bgm_img
[if exp="tf.config_num_bgm == 0"]
	[image layer="0" name="bgmvol" storage="&tf.img_path + 'mute_active.png'" x="&tf.config_x[0]" y="224"]
[else]
	[image layer="0" name="bgmvol" storage="&tf.btn_path_on" x="&tf.config_x[tf.config_num_bgm]" y="&tf.config_y[0]"]
[endif]

[return]


*load_se_img
[if exp="tf.config_num_se == 0"]
	[image layer="0" name="sevol" storage="&tf.img_path + 'mute_active.png'" x="&tf.config_x[0]" y="304"]
[else]
	[image layer="0" name="sevol"  storage="&tf.btn_path_on" x="&tf.config_x[tf.config_num_se]" y="&tf.config_y[1]"]
[endif]

[return]


*load_ch_img
[image layer="0" name="ch" storage="&tf.btn_path_on" x="&tf.config_x[tf.config_num_ch + 1]" y="&tf.config_y[2]"]

[return]


*load_auto_img
[image layer="0" name="auto"  storage="&tf.btn_path_on" x="&tf.config_x[tf.config_num_auto + 1]" y="&tf.config_y[3]"]

[return]

*load_skip_img
[if exp="tf.text_skip == 'ON'"]
	[image layer="0" name="unread_on" storage="&tf.img_path + 'skipon_active.png'" x="351" y="530"]
[else]
	[image layer="0" name="unread_off" storage="&tf.img_path + 'skipoff_active.png'" x="462" y="530"]
[endif]

[return]

*load_screen_img
[if exp="tf.screen_size == 'full'"]
	[image layer="0" name="screen_full" storage="&tf.img_path + 'full_active.png'" x="785" y="530"]
[else]
	[image layer="0" name="screen_window" storage="&tf.img_path + 'window_active.png'" x="907" y="530"]
[endif]

[return]
