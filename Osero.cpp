#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#define scanf scanf_s
#define BOARD 8 //�{�[�h�̑傫��
//��Ԓ�`
#define NONE 0 
#define BLACK 1 
#define WHITE -1
//��Փx��`
#define EASY 1
#define NORMAL 2
#define HARD 3
//���ՏI�Ղ̎萔
#define MIDDLE 15
#define FINISH 48
//�T���萔
#define N_SEARCH 5
#define H_SEARCH 7
//���x����`
#define MIDDLE 10 
#define FINISH 48    
//�e�[�u������
const int Value[BOARD][BOARD] = {
	{ 50,-10,  4, -1, -1,  4,-10, 50, },
	{ -10,-15, -1, -3, -3, -1,-15,-10, },
	{ 4, -1,  2, -1, -1,  2, -1,  4, },
	{ -4, -3, -1,  0,  0, -1, -3, -4, },
	{ -1, -3, -1,  0,  0, -1, -3, -4, },
	{ 4, -1,  2, -1, -1,  2, -1,  4, },
	{ -10,-15, -1, -3, -3, -1,-15,-10, },
	{ 50,-10,  4, -1, -1,  4,-10, 50, }
};
//�΂̕ω��̏���u���\����
typedef struct info
{
	int x, y;           //�΂̏ꏊ
	int point;          //�Ԃ����΂̐�
	int position[30];   //�΂̂Ђ�����Ԃ����ꏊ
}Nodo;
int Number;   //�萔
int board[BOARD][BOARD];  //�{�[�h
int mode;                  //���[�h
						   //�ړ���
int vector_y[] = { -1,-1,0,1,1,1,0,-1 };
int vector_x[] = { 0,1,1,1,0,-1,-1,-1 };
//�{�[�h�̏�����
void InitBoard(void)
{
	int x, y;
	//�{�[�h��������
	for (y = 0; y<BOARD; y++)
	{
		for (x = 0; x<BOARD; x++)
		{
			board[y][x] = NONE;
		}
	}
	//�{�[�h��̏����̍��΂Ɣ��΂̏ꏊ
	board[BOARD / 2 - 1][BOARD / 2 - 1] = BLACK;
	board[BOARD / 2][BOARD / 2] = BLACK;
	board[BOARD / 2 - 1][BOARD / 2] = WHITE;
	board[BOARD / 2][BOARD / 2 - 1] = WHITE;
	Number = 0;     //�萔�̏�����
}
//nodo�̏�����
void initNodo(Nodo* nodo, int x, int y)
{
	nodo->x = x;
	nodo->y = y;
	nodo->point = 0;
	for (int i = 0; i <30; i++) nodo->position[i] = 0;
}
//�{�[�h�\��
void Display(void) {
	int x, y;
	//�Ֆʂɔԍ�������
	for (x = 0; x<BOARD; x++) printf("%2d", x + 1);
	printf("\n");
	for (y = 0; y<BOARD; y++) {
		printf("%d", (y + 1));
		for (x = 0; x<BOARD; x++)
		{
			//�{�[�h�オ0���P��-1������ȊO�ł����error��Ԃ�
			if (board[y][x] == NONE)
				printf("�E");
			else if (board[y][x] == BLACK)
				printf("��");
			else if (board[y][x] == WHITE)
				printf("��");
		}
		printf("\n");
	}
}
//��Փx�����߂�
void InputMode() {
	int lv, check;
	mode = 0;
	while (1) {
		printf("��Փx�����߂Ă�������\n1:easy,2:normal,3;hard\n");
		if (scanf("%d", &lv) == 0)//���l���Ȃ���΃N���A
		{
			scanf("%*[^\n]%*c");
			printf("���̓G���[\n");
		}
		if (lv == EASY) {
			printf("easy���[�h�ŊJ�n���܂�\n");
			mode = 1;
			break;
		}
		else if (lv == NORMAL) {
			printf("noraml���[�h�ŊJ�n���܂�\n");
			mode = 2;
			break;
		}
		else if (lv == HARD) {
			while (1)
			{
				printf("�{���ɋX�����ł���(1;yes,2;no)�H\n");
				//1���������Γ�Փx���߂ɖ߂�0���������΂��̂܂ܑ��s
				if (scanf("%d", &check) == NULL)   //���l���Ȃ���΃N���A
				{
					scanf("%*[^\n]%*c");
				}
				else if (check == 1) {
					printf("�J�n���܂�\n");
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
//��s(��)��s(��)�����߂�
int InputTurn()
{
	int ch;
	while (1)
	{
		printf("��s(��)����s(��)���I��ł��������B\n1:��s(��),2:��s(��)\n");
		if (scanf("%d", &ch) == 0)//���l���Ȃ���΃N���A
		{
			scanf("%*[^\n]%*c");
			printf("���̓G���[\n");
		}
		if (ch == 1)
		{
			return 1;
			break;
		}
		else if (ch == 2)
		{
			return 0;
			break;
		}
	}
}
//�Ђ�����Ԃ����邩������
int VectorLook(int x, int y, int turn, int vec)
{
	int flag = 0;
	while (1) {
		x += vector_x[vec];
		y += vector_y[vec];
		//�{�[�h�O�ŏI��
		if (x<0 || y<0 || x >BOARD - 1 || y > BOARD - 1)return 0;
		//�󂢂Ă���}�X������ΏI��
		if (board[y][x] == NONE)return 0;
		//����̋����΃t���O������
		if (board[y][x] == (turn ? BLACK : WHITE)) {
			flag = 1;
			continue;
		}
		//�t���O�����Ă΃��[�v�I��藧���Ă��Ȃ��ꍇ���s
		if (flag == 1)
		{
			break;
		}
		return 0;
	}
	return 1;
}
//�u�����Ƃ��ł��邩
int Check(int x, int y, int turn)
{
	int vector;
	//�Ђ�����Ԃ邩�H
	for (vector = 0; vector<8; vector++) {
		if (VectorLook(x, y, turn, vector) == 1)return 1;
	}
	return 0;
}
//���Ԃ�����
void Turn(Nodo* nodo, int turn, int vec) {
	int x = nodo->x;
	int y = nodo->y;
	int i = nodo->point;
	while (1) {
		x += vector_x[vec];
		y += vector_y[vec];
		//�������g���ǂ����������g�ł���Ώ����I��
		if (board[y][x] == (turn ? WHITE : BLACK)) {
			break;
		}
		board[y][x] = (turn ? WHITE : BLACK);	//�������g�łȂ��Ȃ�Ђ�����Ԃ�
		nodo->position[i] = x + y*BOARD;      //�L������
		i++;
	}
	nodo->position[nodo->point = i] = 0;
}
//���͂��痠�Ԃ�����
int InputTurn(Nodo* nodo, int turn) {
	int vector, flag = 0;

	//�S�Ė��܂��Ă���΃Q�[���I��
	if (board[nodo->y][nodo->x] != NONE)return 0;
	//�S�Ă̕������m�F
	for (vector = 0; vector<8; vector++) {
		//�Ђ�����Ԃ镨������Η��Ԃ�
		if (VectorLook(nodo->x, nodo->y, turn, vector) == 1) {
			//���Ԃ�����
			Turn(nodo, turn, vector);
			flag = 1;
		}
	}
	if (flag == 1) {
		//�t���O�������Ă���΃t���O�̂������ꏊ�ɒu��
		board[nodo->y][nodo->x] = (turn ? WHITE : BLACK);
		return 1;
	}
	return 0;
}
//����
void Input(int turn)
{
	int x, y, re;
	while (1) {
		//����
		printf("�P�`�W�܂ł̊Ԃ�x������͂��Ă��������B>");
		if (scanf("%d", &x) == 0)//���l���Ȃ���΃N���A
		{
			scanf("%*[^\n]%*c");
			printf("���̓G���[\n");
			continue;
		}
		printf("1�`8�܂ł̊Ԃł�������͂��Ă��������B>");
		if (scanf("%d", &y) == 0)//���l���Ȃ���΃N���A
		{
			scanf("%*[^\n]%*c");
			printf("���̓G���[\n");
			continue;
		}
		else if (x>BOARD || x <= 0 || y>BOARD || y <= 0) {
			printf("�͈͓��œ��͂��Ă�������\n");
			x = 0; y = 0;
			continue;
		}
		//�u���邩
		if (Check(x - 1, y - 1, turn) == 1) {
			Nodo nodo;
			initNodo(&nodo, x - 1, y - 1);              //�����m�[�h�ɂ���1�������l�Ƃ�y�ɂP�������l������
			InputTurn(&nodo, turn);                      //���Ԃ�����
			Number++;                                  //�萔���P����
			break;
		}
		else printf("���Ԃ��܂���ł����B\n����̐΂����Ԃ���ꏊ�ɂ����Ă��������B\n");
		x = 0; y = 0;
	}

}
//�I�Z���̏�Ԃ�����
void Reverse(Nodo nodo)
{
	int i = 0;
	//�m�[�h�|�W�V�����ɐ��l�������Ă���΂��̃|�W�V������-1���|���Ė߂�
	while (nodo.position[i]> 0) {
		int x = nodo.position[i] % 8;
		int y = nodo.position[i] / 8;
		board[y][x] *= -1;
		i++;
		if (nodo.position[i] == 0) {
			board[nodo.y][nodo.x] = NONE;
			break;
		}
	}
}
//�]������ꏊ
int ValuePlace()
{
	int x, y, value = 0;
	for (y = 0; y < BOARD; y++)
		for (x = 0; x < BOARD; x++)
			value += board[y][x] * Value[x][y];
	return(-value);
}
//�肪����ꏊ�̐��ŔՖʕ]��
int ValueDropDown(int turn)
{
	int x, y, value = 0;
	for (y = 0; y < BOARD; y++)
		for (x = 0; x < BOARD; x++)
			if (Check(x, y, turn))value += 1;     //�u����Ε]���ɂP����
	if (turn != 0)return(3 * value);
	else return(-3 * value);
}
//�Ֆʂ𑊎�Ƃ̐΂̐��ŕ]������
int ValueBoardNum() {
	int x, y, value = 0;
	for (y = 0; y < BOARD; y++)
		for (x = 0; x < BOARD; x++)value += board[y][x];
	return(value*-1);
}
//�Ֆʕ]��
int ValueBoard(int turn) {
	int value = 0;
	if (Number <= MIDDLE)          //����
	{
		value += ValuePlace();
		value += ValueDropDown(turn);
	}
	else if (Number <= FINISH)     //����
	{
		value += ValuePlace();
		value += ValueDropDown(turn);
	}
	else value += ValueBoardNum();  //�I��

	if (turn == 1)return(value);
	else return(-value);

}
//��-���@�ŒT���ōőP�̍��T��
int AB(bool flag, int lv, bool put, int turn, int mode, int al, int be)
{
	int temp, x, y, vest_x, vest_y;
	bool flagput = false;
	Nodo nodo;
	//���x�����O�̂Ƃ��O��Ԃ�
	if (lv == 0) {
		//���[�h�P�ł͂O�Q�A�R�ł͎v�l������
		if (mode == 1) return 0;
		else if (mode == 2 || mode == 3) return(ValueBoard(turn));
	}
	//�t���O�����Ă�-9999�o���Ȃ����9999
	if (flag)al = -9999;
	else al = 9999;
	//�Ֆʕ]��
	for (y = 0; y < BOARD; y++)
	{
		for (x = 0; x < BOARD; x++)
		{
			if (Check(x, y, turn) == 1)
			{
				flagput = true;
				initNodo(&nodo, x, y);              //�L������
				InputTurn(&nodo, turn);             //�΂�u��
				turn = (turn + 1) % 2;              //��Ԃ�������
				temp = AB(!flag, lv - 1, true, turn, mode, al, be); //��Ԃ��v���C���[�֕ς������߃t���O��false��
				Reverse(nodo);                                  //���ɖ߂�
				turn = (turn + 1) % 2;
				//�t���O������temp�̕������]�����傫���Ȃ邩�t�ł���΃x�X�g�ɂ����
				if (flag) {
					if (temp >= al)
					{
						al = temp;
						vest_x = x;
						vest_y = y;
					}
					//���̕��������傫����΃�������
					if (al > be)return(al);
				}
				else
				{
					if (temp <= be)
					{
						be = temp;
						vest_x = x;
						vest_y = y;
					}
					if (al > be)return(be);
				}
			}
		}
	}
	//���x�����T���ő�ł���΂��̒l������
	if (flagput)
	{
		if ((lv == N_SEARCH&&mode == 1) || (lv == N_SEARCH&&mode == 2))return(vest_x + vest_y*BOARD);
		else if (lv == H_SEARCH&&mode == 3)return(vest_x + vest_y*BOARD);
		else if (flag)return(al);
		else return(be);
	}
	else if (!put&&mode == 1) return 0;
	else if (!put&&mode == 2)return (ValueBoard(turn));
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
	Nodo nodo;
	if (mode == 1 || mode == 2) {
		y = AB(true, N_SEARCH, true, turn, mode, -9999, 9999);   //�őP��
	}
	else if (mode == 3)
	{
		y = AB(true, H_SEARCH, true, turn, mode, -9999, 9999);   //�őP��
	}
	if (0 > y || y >= BOARD*BOARD)
	{
		turn = (turn + 1) % 2;
		return;
	}
	x = y % BOARD;
	y = y / BOARD;
	initNodo(&nodo, x, y);                                //�����m�[�h��x��y�̒l������
	InputTurn(&nodo, turn);                               //���Ԃ� 
	Number++;                                             //�萔���P���₷
}
//�Q�[���I���m�F�̏���
int CheckEnd(int turn)
{
	int x, y;
	//�S�Ă̏ꏊ���m�F
	for (y = 0; y<BOARD; y++)
	{
		for (x = 0; x<BOARD; x++)
		{
			//�����ꏊ������Α��s
			if (board[y][x] == NONE &&Check(x, y, turn) == 1)return 0;
		}
	}
	//�ꏊ��������΍����݂̃v���C���[����サ�čX�ɒT��
	turn = (turn + 1) % 2;
	for (y = 0; y<BOARD; y++)
	{
		for (x = 0; x<BOARD; x++)
		{
			//�����ꏊ������Α��s
			if (board[y][x] == NONE &&Check(x, y, turn) == 1)return 1;
		}
	}
}
//���҂̔���
void Win() {
	int x, y, p = 0, n = 0;
	//�΂𐔂���
	for (y = 0; y<BOARD; y++) {
		for (x = 0; x<BOARD; x++) {
			if (board[y][x] == BLACK) p++;
			else if (board[y][x] == WHITE)n++;
		}
	}
	//���ҕ\��
	if (p<n)printf("�v���C���[�̏����ł��B\n���߂łƂ��������܂��B\n");
	else if (p>n)printf("�v���C���[�̕����ł�\n");
	else printf("���������ł�\n");
}

int main() {
	int turn = 0, i_turn,Ai_turn;
	InitBoard();       //�������֐�
	 //��s��s�m�F
	if (InputTurn() == 1)
	{
		i_turn = 0;
		Ai_turn = 1;
	}
	else
	{
		i_turn = 1;
		Ai_turn = 0;
	}
	//���[�h�֐�
	InputMode();
	//���[�h���I�΂�Ă�����J�n
	if (mode == 1 || mode == 2 || mode == 3)
	{
		while (turn < 2) {
			if (turn == i_turn) {
				printf("�M���̔Ԃł�\n");
				Display();                //�\���֐�
			}
			else printf("���s���E�E�E\n");
			if (turn == i_turn) Input(turn);    //���͊֐�
			else if (turn == Ai_turn) {
				AI(Ai_turn, mode);            //�ȒP��AI�֐�

			}
			turn = (turn + 1) % 2;             //���
	      //�����G���h�֐����P�Ȃ�΃p�X�Q�Ȃ�ΏI��
			if (CheckEnd(turn) == 1) {
				printf("�p�X\n");
				turn = (turn + 1) % 2;
			}
			else if (CheckEnd(turn) == 2) {
				printf("�Q�[���I��\n");
				turn = 2;
			}

		}
		Win();                      //�����֐�
		return 0;
	}
}