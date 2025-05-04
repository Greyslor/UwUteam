<?php

    class Gato{
        public $db="game.db";

        public  $p1, // username
                $p2, // username2
                $actual,
                $round,
                $score1,
                $score2,
                $board; // array

        public function init()
        {
            $this->board = array(0,0,0,0,0,0,0,0,0);
            
            $this->p1="id1";
            $this->p2="id2";
            $this->actual=0;
            $this->round=0;
            $this->score1=0;
            $this->score2=0;

            $this->saveDb();
        }
        
        public function saveDb ()
        {
            $file = fopen($this->db, "w") or die("error");
            $strData = json_encode($this->toJson());
            fwrite($file, $strData);
            fclose($file);
        }

        public function toJson ()
        {
            $data = array(
                "p1"=>$this->p1,
                "p2"=>$this->p2,
                "actual"=>$this->actual,
                "round"=>$this->round,
                "score1"=>$this->score1,
                "score2"=>$this->score2,
                "board"=>$this->board
            );

            return $data;
        }

        public function loadDb ()
        {
            $file = fopen($this->db, "r") or die ("error");
            $strData = fread($file,filesize($this->db));
            $data = json_decode($strData);
            
            $this->p1 = $data->p1;
            $this->p2 = $data->p2;
            $this->actual = $data->actual;
            $this->round = $data->round;
            $this->score1 = $data->score1;
            $this->score2 = $data->score2;
            $this->board = $data->board;
        }

        public function toString()
        {
            echo "".
            "p1:".$this->p1."<br/>".
            "p2:".$this->p2."<br/>".
            "actual:".$this->actual."<br/>".
            "round:".$this->round."<br/>".
            "score1:".$this->score1."<br/>".
            "score2:".$this->score2."<br/>".
            "board:";
            var_dump($this->board);
            
        }

        public function getPlayer($id)
        {
            if ($id == $this->p1)
                return 1;
            elseif ($id == $this->p2)
                return 2;
            else
                return 0;
        }

        public function getStatus ()
        {
            $data = array(
                "actual"=>$this->actual,
                "round"=>$this->round,
                "score1"=>$this->score1,
                "score2"=>$this->score2,
                "board"=>$this->board,
            );

            return json_encode($data);
        }

        public function turn($id, $pos) // pos en formato de array unidimensional
        {
            if(($this->actual % 2 ) == ($this->getPlayer($id) -1)){
                if($pos <= 8 & $pos >= 0){
                    if ( $this->board[$pos] == 0 ) // pos vacía
                    {
                        //guardar pos
                    $this->board[$pos] = $this->getPlayer($id);
                        $this->actual++;
                        return $this->isWin();
                    }else{ // error
                        return "error";
                    }
                }else{
                    return "box not found";
                }
            }else{
                return "not your turn";
            }

        }

        public function isWin ()
        {
            if($this->board[0] == $this->board[1] & $this->board[1] == $this->board[2] & $this->board[0] != 0|| 
            $this->board[3] == $this->board[4] & $this->board[4] == $this->board[5] & $this->board[3] != 0|| 
            $this->board[6] == $this->board[7] & $this->board[7] == $this->board[8] & $this->board[6] != 0|| 

            $this->board[0] == $this->board[3] & $this->board[3] == $this->board[6] & $this->board[0] != 0|| 
            $this->board[1] == $this->board[4] & $this->board[4] == $this->board[7] & $this->board[1] != 0|| 
            $this->board[2] == $this->board[5] & $this->board[5] == $this->board[8] & $this->board[2] != 0||
            
            $this->board[0] == $this->board[4] & $this->board[4] == $this->board[8] & $this->board[0] != 0|| 
            $this->board[2] == $this->board[4] & $this->board[4] == $this->board[6] & $this->board[2] != 0){
                if(($this->actual % 2 ) == 0){
                    $this->score2 ++;
                    $this->round ++;
                    return "player2 wins";
                }else{
                    $this->score1 ++;
                    $this->round ++;
                    return "player1 wins";
                }
            }else if($this->actual == 9){
                $this->round ++;
                return "tie";
            }else{
                return"OK";
            }
        }

        public function newGame ()
        {
            $file = fopen($this->db, "r") or die ("error");
            $strData = fread($file,filesize($this->db));
            $data = json_decode($strData);

            $this->board = array(0,0,0,0,0,0,0,0,0);
            
            $this->p1="id1";
            $this->p2="id2";
            $this->actual=0;
            $this->round = $data->round;
            $this->score1 = $data->score1;
            $this->score2 = $data->score2;

            $this->saveDb();
        }
    }

    $gato = new Gato();

    if( !empty($_GET["action"]) )
    {
        $action = $_GET["action"];
    }
    else
    {
        $action = 0;
    }

    switch($action)
    {
        case 0: // empty
            echo "empty";
        break;

        case 1:
            $gato->init();
            $gato->loadDb();
            $gato->toString();
        break;

        case 2:
            $gato->loadDb();
            echo $gato->getStatus();
        break;

        case 3: // turn
            if( !empty($_GET["id"]) ) // user
            {
                $id = $_GET["id"];
            }
            else
            {
                $id = 0;
            }
            
            if( !empty($_GET["pos"]) ) // user
            {
                $pos = $_GET["pos"]-1;
            }
            else
            {
                $pos = -1;
            }

            $gato->loadDb();
            echo $gato->turn($id, $pos);
            $gato->saveDb();
        break;
        case 4: // newgame
            $gato->newGame();
        break;

        default:
            echo "No Control";
        break;
    }

?>