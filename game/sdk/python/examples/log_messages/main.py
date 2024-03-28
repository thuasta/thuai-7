import pathlib
import sys

sys.path.append(
    str(pathlib.Path(__file__).parent.parent.parent.resolve()))

from src.novelcraft.sdk import *

def main():
    logger = get_logger()
    logger.debug('Debug message')
    logger.info('Info message')
    logger.warn('Warning message')
    logger.error('Error message')

if __name__ == '__main__':
    main()
