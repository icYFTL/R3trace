CREATE OR REPLACE PROCEDURE public.delete_user(
    userUid UUID,
    soft BOOLEAN,
    INOUT msg TEXT default '',
    INOUT result BOOLEAN default false
)
LANGUAGE plpgsql
AS $$
DECLARE
    teamUid UUID;
BEGIN
	
    -- Check if the user exists
    IF NOT EXISTS (SELECT 1 FROM users WHERE uid = userUid) THEN
        msg := 'User not found';
        result := false;
        RETURN;
    END IF;

    -- Retrieve the team UID
    SELECT team_uid INTO teamUid
    FROM users u
    JOIN teams t ON t.uid = u.team_uid
    WHERE u.uid = userUid;

    -- Delete or update teams table based on the condition
    IF teamUid IS NOT NULL THEN
        IF (SELECT COUNT(*) FROM users WHERE team_uid = teamUid) = 1 THEN
            IF soft = false THEN
                DELETE FROM teams WHERE uid = teamUid;
            ELSE
                UPDATE teams t
                SET deleted = true
                WHERE t.uid = teamUid;
            END IF;
        END IF;
    END IF;

    -- Delete or update users table based on the condition
    IF soft = false THEN
        DELETE FROM users WHERE uid = userUid;
    ELSE
        UPDATE users
        SET deleted = true
        WHERE uid = userUid;
    END IF;

    -- Set the success message and result
    msg := 'Ok';
    result := true;
END;
$$;
