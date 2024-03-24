;===========================================================
; CGモード画面作成
;===========================================================

; このファイルはscenarioフォルダ内に配置、
; image フォルダ内に append_theme フォルダを配置して使用します
; 必要に応じて枚数やページの増減をおこなってください

;-----------------------------------------------------------
*start
;-----------------------------------------------------------
; 初期化
[layopt layer=message0 visible=false]
[layopt layer=0 visible=true]
[layopt layer=1 visible=true]
[hidemenubutton]

[clearfix]
[cm]

; ギャラリーモードの背景読込み
[bg storage=../image/append_theme/gallery_bg.png time=300]

[iscript]

tf.page              = 0   // ページ番号
tf.selected_cg_image = []  // 選択したCGの差分を格納した配列変数
tf.cg_index          = 0   // 上の配列の要素番号

tf.cg_posx = [152, 480, 808]; // サムネイルのX座標
tf.cg_posy = [220, 408]; // サムネイルのY座標
tf.cg_thumbnail_width  = 320; // サムネイルの幅
tf.cg_thumbnail_height = 180; // サムネイルの高さ

[endscript]

; ページネーション（ページ数が変わるときはtextの中身を修正してね）
[macro name="pagination"]
  [layopt layer="0" visible="true"]
  [free layer="0" name="pagination" time="1"]
  [ptext layer="0" name="pagination" text="&tf.page + 1 + '/3'" x="0" y="645" size="18" color="0xdadae5" width="1280" align="center"]
  [endmacro]

[jump target=*cgpage]

;-----------------------------------------------------------
*cgpage
;-----------------------------------------------------------
[cm]

; ギャラリーモード終了
[button hint="ギャラリー画面を終了する" graphic=append_theme/gallery_close.png enterimg=append_theme/gallery_close2.png target=*backtitle x=1192 y=24]

; tf.page変数を利用して個別閲覧ボタン作成ラベルにジャンプします
[jump target="& 'page_' + tf.page "]

;-------------------------------------------------------
*page_0
;-------------------------------------------------------

; CG閲覧モード画面1ページ目

; graphic には表示する画像のファイル名
; thumb にはサムネイルとして表示する画像のファイル名（記述がなければgraphicのファイルを指定）

; 一段目
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[0]" y="&tf.cg_posy[0]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[1]" y="&tf.cg_posy[0]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[2]" y="&tf.cg_posy[0]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]

; 二段目
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[0]" y="&tf.cg_posy[1]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[1]" y="&tf.cg_posy[1]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[2]" y="&tf.cg_posy[1]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]

; 次ページへ
[button hint="次のページ" graphic=append_theme/gallery_next.png enterimg=append_theme/gallery_next2.png target=*nextpage x=1188 y=360]

; ページネーション
[pagination]

; 共通処理にジャンプ
[jump target=*common]

;-------------------------------------------------------
*page_1
;-------------------------------------------------------
; CG閲覧モード画面2ページ目

; 一段目
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[0]" y="&tf.cg_posy[0]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[1]" y="&tf.cg_posy[0]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[2]" y="&tf.cg_posy[0]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]

; 二段目
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[0]" y="&tf.cg_posy[1]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[1]" y="&tf.cg_posy[1]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[2]" y="&tf.cg_posy[1]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]


; 前のページへ
[button hint="前のページ" graphic=append_theme/gallery_prev.png enterimg=append_theme/gallery_prev2.png target=*backpage x=56 y=360]

; 次のページへ
[button hint="次のページ" graphic=append_theme/gallery_next.png enterimg=append_theme/gallery_next2.png target=*nextpage x=1188 y=360]

[pagination]

; 共通処理にジャンプ
[jump target=*common]

;-------------------------------------------------------
*page_2
;-------------------------------------------------------
; CG閲覧モード画面3ページ目

; 一段目
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[0]" y="&tf.cg_posy[0]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[1]" y="&tf.cg_posy[0]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[2]" y="&tf.cg_posy[0]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]

; 二段目
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[0]" y="&tf.cg_posy[1]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[1]" y="&tf.cg_posy[1]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]
[cg_image_button graphic="" thumb="" no_graphic="../image/append_theme/lock.png" x="&tf.cg_posx[2]" y="&tf.cg_posy[1]" width="&tf.cg_thumbnail_width" height="&tf.cg_thumbnail_height"]


; 前ページへ
[button hint="前のページ" graphic=append_theme/gallery_prev.png enterimg=append_theme/gallery_prev2.png target=*backpage x=56 y=360]

[pagination]

; 共通処理にジャンプ
[jump target=*common]

;-------------------------------------------------------
*common
;-------------------------------------------------------

; 停止

[s]


;-----------------------------------------------------------
*backtitle
;-----------------------------------------------------------
; タイトルに戻る処理

; 使用したレイヤーをすべて消去
[cm]
[freeimage layer=0]
[freeimage layer=1]

; 別のシナリオにジャンプする場合はここを変更
[jump storage=title.ks]


;-----------------------------------------------------------
*nextpage
;-----------------------------------------------------------
; 次のページに移る処理

; 一時変数 tf.page を増加させたうえで *cgpage へ
[eval exp=tf.page++]
[jump target=*cgpage]

;-----------------------------------------------------------
*backpage
;-----------------------------------------------------------
; 前のページに移る処理

; 一時変数 tf.page を減少させたうえで *cgpage へ
[eval exp=tf.page--]
[jump target=*cgpage]

;-----------------------------------------------------------
*no_image
;-----------------------------------------------------------
; 未解放のCGをクリックしたときの処理
[jump target=*cgpage]

;-----------------------------------------------------------
*clickcg
;-----------------------------------------------------------
; 解放済みのCGをクリックしたときの処理

; フリーレイヤーとレイヤー１(back)を解放します
[cm]
[freeimage layer=1 page=back]

; 一時変数 tf.cg_index に 0 をぶち込みます
[eval exp="tf.cg_index = 0"]

;-------------------------------------------------------
*cg_next_image
;-------------------------------------------------------
; CGを見ていきます。
; 見るべきCGがひとつしかない場合は、それだけ見たあと *cgpage に戻ります。
; 見るべきCGが複数ある場合(差分がある場合)は、
; 再帰的にこのラベルに飛び直して次のCGを見ていきます。

; 一時変数 tf.storage に表示すべきCGのstorageを代入します
[iscript]
tf.storage = tf.selected_cg_image[tf.cg_index];
[endscript]

; CGを表示してクリックを待ちます。
[freeimage layer=1 page=back]
[image     layer=1 page=back storage=&tf.storage folder=bgimage width=1280 height=720]
[trans     layer=1 time=700]
[wt]
[l]

; クリックされたら
; 一時変数 tf.cg_index (差分画像がある場合の画像番号)を1増加させます。
[eval exp=tf.cg_index++]

; まだ表示すべき差分画像が残っているなら、このラベルに飛びなおします。
[if exp=" tf.selected_cg_image.length > tf.cg_index "]
  [jump target=cg_next_image]

[else]
  [freeimage layer=1 page=back]
  [freeimage layer=1 page=fore time=700]
  [jump target=*cgpage]

[endif]
