#include<iostream>
#define BOARD 8 //ボードの大きさ
//状態定義
#define NONE 0 
#define BLACK 1 
#define WHITE -1
//難易度定義
#define EASY 1
#define NORMAL 2
#define HARD 3
//中盤終盤の手数
#define MIDLE 15
#define FINISH 48
//探索手数
#define N_SEARCH 3
#define H_SEARCH 5  
//テーブル生成
const int Value[BOARD][BOARD] = {
	{ 50,-10,  4, -1, -1,  4,-10, 50, },
	{ -10,-15, -1, -3, -3, -1,-15,-10, },
	{ 4, -1,  2, -1, -1,  2, -1,  4, },
	{ -4, -3, -1,  0,  0, -1, -3, -4, },
	{ -1, -3, -1,  0,  0, -1, -3, -1, },
	{ 4, -1,  2, -1, -1,  2, -1,  4, },
	{ -10,-15, -1, -3, -3, -1,-15,-10, },
	{ 50,-10,  4, -1, -1,  4,-10, 50, }
};
//石の変化の情報を置く構造体
typedef struct
{
	int x, y;           //石の場所
	int point;          //返った石の数
	int position[64];   //石のひっくり返った場所
}Node;
int Number;                  //手数
int board[BOARD][BOARD];     //ボード
int mode;                   //モード
//ターン
int Ai_turn;    //AI
int i_turn;     //Player
//移動量
int vector_y[] = { -1,-1,0,1,1,1,0,-1 };
int vector_x[] = { 0,1,1,1,0,-1,-1,-1 };

using namespace std;

//ボードの初期化
void InitBoard(void)
{
	int x, y;
	//ボードを初期化
	for (y = 0; y<BOARD; y++)
	{
		for (x = 0; x<BOARD; x++)
		{
			board[y][x] = NONE;
		}
	}
	//ボード上の初期の黒石と白石の場所
	board[BOARD / 2 - 1][BOARD / 2 - 1] = BLACK;
	board[BOARD / 2][BOARD / 2] = BLACK;
	board[BOARD / 2 - 1][BOARD / 2] = WHITE;
	board[BOARD / 2][BOARD / 2 - 1] = WHITE;
	Number = 0;     //手数の初期化
}
//ノードの初期化
void InitNode(Node* node, int x, int y)
{
	node->x = x;
	node->y = y;
	node->point = 0;
	for (int i = 0; i <64; i++) node->position[i] = 0;
}
//ボード表示
void Display(void) {
	int x, y;
	//盤面に番号をつける
	for (x = 0; x<BOARD; x++) printf("%2d", x + 1);
	printf("\n");
	for (y = 0; y<BOARD; y++) {
		printf("%d", (y + 1));
		for (x = 0; x<BOARD; x++)
		{
			//ボード上が0であれば・、１であれば黒、-1であれば白
			if (board[y][x] == NONE)
				printf("・");
			else if (board[y][x] == BLACK)
				printf("●");
			else if (board[y][x] == WHITE)
				printf("○");
		}
		printf("\n");
	}
}
//難易度を決める
void InputMode() {
	int lv, check;
	mode = 0;
	while (1) {
		printf("難易度を決めてください\n1:easy,2:normal,3;hard\n");
		if ( !(cin >> lv)) {
			printf("入力エラー\n");
			cin.clear();
			cin.ignore(10000, '\n');
		}
		if (lv == EASY) {
			printf("easyモードで開始します\n");
			mode = 1;
			break;
		}
		else if (lv == NORMAL) {
			printf("noramlモードで開始します\n");
			mode = 2;
			break;
		}
		else if (lv == HARD) {
			while (1)
			{
				printf("本当に宜しいですね(1;yes,2;no)？\n");
				if (!(cin >> check)) {
					printf("入力エラー\n");
					cin.clear();
					cin.ignore(10000,'\n');
				}
				 if (check == 1) {
					printf("開始します\n");
					mode = 3;
					break;
				}
				else if (check == 2) break;
				else continue;
			}
			if (check == 1) break;
			else if (check == 2) continue;
		}

	}
}
//先攻(白)後攻(黒)か決める
void InputTurn()
{
	int ch;
	while (1)
	{
		printf("先攻(白)か後攻(黒)か選んでください。\n1:先行(白),2:後行(黒)\n");
		if (!(cin >> ch)) {
			printf("入力エラー\n");
			cin.clear();
			cin.ignore(10000,'\n');
		}
		//先攻が選ばれれば白、後攻が選ばれれば黒
		if (ch == 1)
		{
			i_turn = 0;
			Ai_turn = 1;
			break;
		}
		else if (ch == 2) {
			i_turn = 1;
			Ai_turn = 0;
			break;
		}
	}
}
//ひっくり返る駒があるかを見る
int VectorLook(int x, int y, int turn, int vec)
{
	int flag = 0;
	while (1) {
		x += vector_x[vec];
		y += vector_y[vec];
		//ボード外で終了
		if (x<0 || y<0 || x >BOARD - 1 || y > BOARD - 1)return 0;
		//空いているマスがあれば終了
		if (board[y][x] == NONE)return 0;
		//相手の駒があればフラグが立つ
		if (board[y][x] == (turn ? BLACK : WHITE)) {
			flag = 1;
			continue;
		}
		//フラグが立てばループ終わり。立っていない場合続行。
		if (flag == 1)break;
		return 0;
	}
	return 1;
}
//置くことができるか
int Check(int x, int y, int turn)
{
	int vector;
	//ひっくり返るか？
	for (vector = 0; vector<8; vector++) {
		if (VectorLook(x, y, turn, vector) == 1)return 1;
	}
	return 0;
}
//裏返す処理
void Turn(Node* node, int turn, int vec) {
	int x = node->x;
	int y = node->y;
	int i = node->point;
	while (1) {
		x += vector_x[vec];
		y += vector_y[vec];
		//自分自身かどうか自分自身であれば処理終了
		if (board[y][x] == (turn ? WHITE : BLACK)) {
			break;
		}
		board[y][x] = (turn ? WHITE : BLACK);	//自分自身でないならひっくり返す
		node->position[i] = x + y*BOARD;      //ひっくり返った場所を記憶する
		i++;
	}
	node->position[node->point = i] = 0;
}
//入力から裏返す判定
int InputTurn(Node* node, int turn) {
	int vector, flag = 0;

	//全て埋まっていればゲーム終了
	if (board[node->y][node->x] != NONE)return 0;
	//全ての方向を確認
	for (vector = 0; vector<8; vector++) {
		//ひっくり返る物があれば裏返す
		if (VectorLook(node->x, node->y, turn, vector) == 1) {
			//裏返す処理
			Turn(node, turn, vector);
			flag = 1;
		}
	}
	if (flag == 1) {
		//フラグが立っていればフラグのたった場所に置く
		board[node->y][node->x] = (turn ? WHITE : BLACK);
		return 1;
	}
	return 0;
}
//入力
void Input(int turn)
{
	int x, y;
	while (1) {
		//入力
		printf("１～８までの間でx軸を入力してください。>");
		if (!(cin >> x)) {
			printf("入力エラー\n");
			cin.clear();
			cin.ignore(10000, '\n');
		}
		printf("1～8までの間でｙ軸を入力してください。>");
		if (!(cin >> y)) {
			printf("入力エラー\n");
			cin.clear();
			cin.ignore(10000, '\n');
		}
		if (x>BOARD || x <= 0 || y>BOARD || y <= 0) {
			printf("範囲内で入力してください\n");
			x = 0; y = 0;
			continue;
		}
		//置けるか
		if (Check(x - 1, y - 1, turn) == 1) {
			Node node;
			InitNode(&node, x - 1, y - 1);              //ノードにｘに1引いた値ととyに１引いた値を入れる
			InputTurn(&node, turn);                      //裏返す処理
			Number++;                                  //手数を１足す
			break;
		}
		else printf("裏返せませんでした。\n相手の石が裏返せる場所においてください。\n");
		x = 0; y = 0;
	}

}
//オセロの状態を元へ
void Reverse(Node node)
{
	int i = 0;
	//ノードポジションに数値が入っていればそのポジションに-1を掛けて戻す、それ以外は0を入れる
	while (node.position[i]> 0) {
		int x = node.position[i] % 8;
		int y = node.position[i] / 8;
		board[y][x] *= -1;
		i++;
		if (node.position[i] == 0) {
			board[node.y][node.x] = NONE;
			break;
		}
	}
}
//評価する場所
int ValuePlace()
{
	int x, y, value = 0;
	//評価する場所を探索しその値をvalueの入れていく
	for (y = 0; y < BOARD; y++)
		for (x = 0; x < BOARD; x++)
			value += board[y][x] * Value[x][y];
	return(-value);
}
//手がある場所の数で盤面評価
int ValueDropDown(int turn)
{
	int x, y, value = 0;
	for (y = 0; y < BOARD; y++)
		for (x = 0; x < BOARD; x++)
			if (Check(x, y, turn))value += 1;     //置ければ評価に１足す
//ターンがAIであれば、3を掛けてそのまま返しそれ以外であればマイナスで返す
	if (turn == Ai_turn)return(3 * value);
	else return(-3 * value);
}
//盤面を相手との石の数で評価する
int ValueBoardNum() {
	int x, y, value = 0;
	for (y = 0; y < BOARD; y++)
		for (x = 0; x < BOARD; x++)value += board[y][x];
	return(value*-1);
}
//盤面評価
int ValueBoard(int turn) {
	int value = 0;
	if (Number <= MIDLE)          //序盤
	{
		value += ValuePlace();
		value += ValueDropDown(turn);
	}
	else if (Number <= FINISH)     //中盤
	{
		value += ValuePlace();
		value += ValueDropDown(turn);
	}
	else value += ValueBoardNum();  //終盤
									//AIターンならばそのまま返しそれ以外ならばマイナスで返す
	if (turn == Ai_turn)return(value);
	else return(-value);

}
//α-β法で探索
int AB(bool flag, int lv, bool put, int turn, int mode, int al, int be)
{
	int temp, x, y, vest_x, vest_y;
	bool flagput = false;
	Node node;
	if (lv == 0) {
		//モード１では０、それ以外のモードでは評価した値を返す
		if (mode == 1) return 0;
		else if (mode == 2 || mode == 3) return(ValueBoard(turn));
	}
	//フラグが立てば-9999経たなければ9999
	if (flag)al = -9999;
	else al = 9999;
	//盤面評価
	for (y = 0; y < BOARD; y++)
	{
		for (x = 0; x < BOARD; x++)
		{
			if (Check(x, y, turn) == 1)
			{
				flagput = true;
				InitNode(&node, x, y);              //記憶する
				InputTurn(&node, turn);             //石を置く
				turn = (turn + 1) % 2;              //手番をかえる
				temp = AB(!flag, lv - 1, true, turn, mode, al, be); //手番をプレイヤーへ変えたためフラグをfalseへ
				Reverse(node);                                  //元に戻す
				turn = (turn + 1) % 2;
				//フラグが立ったときAIの方がα評価より高ければベストにいれる
				if (flag) {
					if (temp >= al)
					{
						al = temp;
						vest_x = x;
						vest_y = y;
					}
					//αの方がβより評価がたかければαを評価値として返す
					if (al > be)return(al);
				}
				else {
					//フラグが立たなかった時、Playerよりβの評価が高ければベストに入れる
					if (temp <= be)
					{
						be = temp;
						vest_x = x;
						vest_y = y;
					}
					//βの方がαより小さかった場合βを評価値として返す
					if (al > be)return(be);
				}
			}
		}
	}
	//フラグが立てば探索が最大になった時のxとyを返すフラグが立たなければ盤面を相手の石の数で評価し、それ以外であれば続行しABの最終の値を返す
	if (flagput)
	{
		//フラグが最大でeasy,normalだった場合ノーマル探索値のx,yを返す。hardだった場合ハード探索値のx,yを返す
		if ((lv == N_SEARCH&&mode == 1) || (lv == N_SEARCH&&mode == 2))return(vest_x + vest_y*BOARD);
		else if (lv == H_SEARCH&&mode == 3)return(vest_x + vest_y*BOARD);
		else if (flag)return(al);
		else return(be);
	}
	else if (!put&&mode == 1) return 0;
	else if (!put && (mode == 2 || mode == 3))return (ValueBoard(turn));
	else
	{
		turn = (turn + 1) % 2;
		temp = AB(!flag, lv - 1, false, turn, mode, al, be);
		turn = (turn + 1) % 2;
		return(temp);
	}
}
//AI
void AI(int turn, int mode)
{
	int x, y;
	Node node;
	//モードがeasy,normalであればノーマルサーチ,hardであればハード探索になる
	if (mode == 1 || mode == 2) {
		y = AB(true, N_SEARCH, true, turn, mode, -9999, 9999);   //最善策
	}
	else if (mode == 3)
	{
		y = AB(true, H_SEARCH, true, turn, mode, -9999, 9999);
	}
	if (0 > y || y >= BOARD*BOARD)
	{
		turn = (turn + 1) % 2;
		return;
	}
	x = y % BOARD;
	y = y / BOARD;
	InitNode(&node, x, y);                                //ノードにxとyの値を入れる
	InputTurn(&node, turn);                               //裏返す 
	Number++;                                             //手数を１増やす
}
//ゲーム終了確認の処理
int End(int turn)
{
	int x, y;
	//全ての場所を確認
	for (y = 0; y<BOARD; y++)
	{
		for (x = 0; x<BOARD; x++)
		{
			//もし場所があれば続行
			if (board[y][x] == NONE &&Check(x, y, turn) == 1)return 0;

		}
	}
	//場所が無ければ今現在のプレイヤーを交代して更に探索
	turn = (turn + 1) % 2;
	for (y = 0; y<BOARD; y++)
	{
		for (x = 0; x<BOARD; x++)
		{
			//もし場所があれば続行
			if (board[y][x] == NONE &&Check(x, y, turn) == 1)return 1;
		}
	}
	return 2;
}
//勝者の判定
void Win() {
	int x, y, b = 0, w = 0;
	//石を数える
	for (y = 0; y<BOARD; y++) {
		for (x = 0; x<BOARD; x++) {
			if (board[y][x] == BLACK) b++;
			else if (board[y][x] == WHITE)w++;
		}
	}
	//勝者表示
	if (b<w)printf("ホワイトの勝利です。\n");
	else if (b>w)printf("ブラックの勝利です\n");
	else printf("引き分けです\n");
}

int main() {
	int turn = 0;
	InitBoard();       //初期化関数
	InputTurn();	   //先行後行確認
	InputMode();      //モード関数
					  //モードが選ばれていたら開始
	if (mode == 1 || mode == 2 || mode == 3)
	{
		while (turn < 2) {
			if (turn == i_turn) {
				printf("貴方の番です\n");
				Display();                //表示関数
			}
			else printf("試行中・・・\n");
			if (turn == i_turn) Input(i_turn);    //入力関数
			else if (turn == Ai_turn) {
				AI(Ai_turn, mode);            //AI関数

			}
			//もしエンド関数が0ならば続行１ならばパス２ならば終了
			if (End(turn) == 0) {
				turn = (turn + 1) % 2;             //交代
			}
			else if (End(turn) == 1) {
				turn = (turn + 1) % 2;
				printf("パス\n");
				turn = (turn + 1) % 2;
			}
			else if (End(turn) == 2) {
				printf("ゲーム終了\n");
				break;
			}

		}
		Win();                      //勝利関数
		return 0;
	}
}
