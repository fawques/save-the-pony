import curses
import requests
import uuid

BASE_URL = 'https://ponychallenge.trustpilot.com/pony-challenge/maze'
MAZE_URL = BASE_URL + '/%s'
DISPLAY_URL = MAZE_URL + '/print'

def get_size(dimension_name):
    dimension = 0
    while dimension < 15 or dimension > 25:
        try:
            dimension = int(input('Input maze %s (15-25):\n' % dimension_name))
        except:
            pass
    return dimension

def get_pony_name():
    return 'Spike'

def get_difficulty():
    difficulty = -1
    while difficulty < 0 or difficulty > 10:
        try:
            difficulty = int(input('Input difficulty (0-10):\n'))
        except:
            pass
    return difficulty

def main():
    maze_id_string = input('Input maze id:\n')
    maze_id = None
    try:
        maze_id = uuid.UUID(maze_id_string)
    except:
        print('Invalid maze id. Creating a new one.')
        maze_width = get_size('width')
        maze_height = get_size('height')
        pony = get_pony_name()
        difficulty = get_difficulty()

        initialResponse = requests.post(BASE_URL, 
            json = {
              "maze-width": maze_width,
              "maze-height": maze_height,
              "maze-player-name": pony,
              "difficulty": difficulty
            })
        if initialResponse.status_code != 200:
            print('could not create maze. Status code: %s' % initialResponse.status_code)
            import pdb; pdb.set_trace()
        else:
            returned_id = initialResponse.json()['maze_id']
            print('Map id is %s' % returned_id)
            maze_id = uuid.UUID(returned_id)
     
    # get the curses screen window
    screen = curses.initscr()
     
    # turn off input echoing
    curses.noecho()
     
    # respond to keys immediately (don't wait for enter)
    curses.cbreak()
     
    # map arrow keys to special values
    screen.keypad(True)

    try:
        while True:
            responseMaze = requests.get(DISPLAY_URL % maze_id)
            screen.addstr(0, 0, responseMaze.text)
            screen.move(0,0)
            nextMove = ''
            char = screen.getch()
            if char == ord('q'):
                break
            elif char == curses.KEY_RIGHT:
                # print doesn't work with curses, use addstr instead
                screen.addstr(0, 0, 'east ')
                nextMove = 'east'
            elif char == curses.KEY_LEFT:
                screen.addstr(0, 0, 'west ')       
                nextMove = 'west'
            elif char == curses.KEY_UP:
                screen.addstr(0, 0, 'north')       
                nextMove = 'north'
            elif char == curses.KEY_DOWN:
                screen.addstr(0, 0, 'south')
                nextMove = 'south'
            elif char == ord(' '):
                screen.addstr(0, 0, 'stay ')
                nextMove = 'stay'

            screen.move(1,0)

            moveResponse = requests.post(MAZE_URL % maze_id,
                json = {'direction': nextMove})
            if moveResponse.status_code != 200:
                raise ValueError()
    finally:
        # shut down cleanly
        curses.nocbreak(); screen.keypad(0); curses.echo()
        curses.endwin()

if __name__ == '__main__':
    main()