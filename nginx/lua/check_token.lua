local http = require "resty.http"
local httpc = http.new()
local json = require "json"

local token = ngx.req.get_headers()["Authorization"]
if not token then
    ngx.log(ngx.ERR, "Token is empty")
    return ngx.exit(ngx.HTTP_UNAUTHORIZED)
end

token = token:gsub("Bearer ", "")

local res, err = httpc:request_uri("http://computantis:7003/secure/user/check_token", {
    method = "POST",
    headers = {
        ["Authorization"] = token
    }
})

if not res then
    ngx.log(ngx.ERR, "Request failed: ", err)
    return ngx.exit(ngx.HTTP_INTERNAL_SERVER_ERROR)
end

if res.status == ngx.HTTP_FORBIDDEN then
    -- ngx.log(ngx.ERR, "Forbi" .. token)
    return ngx.exit(ngx.HTTP_FORBIDDEN)
end

if res.status ~= ngx.HTTP_OK then
    -- ngx.log(ngx.ERR, "Token is not valid: " .. token)
    return ngx.exit(ngx.HTTP_UNAUTHORIZED)
end

local response_body = res.body
local data = json.decode(response_body)

if data.response.banned == true then
    return ngx.exit(ngx.HTTP_FORBIDDEN)
end

if data.response.deleted == true then
    return ngx.exit(ngx.HTTP_FORBIDDEN)
end

ngx.var.identify = data.response.uid
ngx.var.isadmin = data.response.isAdmin

httpc:close()
