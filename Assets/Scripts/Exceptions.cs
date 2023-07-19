using System;

public class GameException : Exception {
    private String msg;

    protected GameException() : base() {}

    public GameException(String c_msg) {
        msg = c_msg;
    }
}


public class NotImplementedException : Exception {
    public NotImplementedException() : base() {}
}