import {getInstance} from '@/services/base.api'
// import store from "@/store";

class AuthService {
    constructor() {
        this.axios_instance = getInstance();
    }

    async login(username, password) {
        return this.axios_instance
            .post('/user/sign_in', {
                username: username,
                password: password
            })
            .then(response => {
                return response.data;
            });
    }
    async register(username, password) {
        return this.axios_instance
            .post('/user/sign_up', {
                username: username,
                password: password
            })
            .then(response => {
                return response.data;
            });
    }

    // checkCode(code) {
    //     return this.axios_instance
    //         .post('/user/check_code', {
    //             'code': code
    //         })
    //         .then(response => {
    //             if (response.data.status) {
    //                 console.log(response.data)
    //                 localStorage.setItem('token', response.data.response.jwtToken); // TODO: Replace that into cookies
    //                 localStorage.setItem('refreshToken', response.data.response.refreshToken);
    //                 return response.data;
    //             }
    //
    //         }).catch(() => {
    //             return false;
    //         });
    // }

    logout() {
        if (localStorage.getItem('token') !== null)
            this.axios_instance.post('/user/logout')
                .catch(err => {
                    console.error(err.toString())
                }).finally(() => {
                localStorage.removeItem('token');
                localStorage.removeItem('refreshToken');
                window.location.replace('/');
            })
    }
    async refreshToken() {
        let refreshToken = localStorage.getItem('refreshToken');
        if (refreshToken !== null) {
            return await this.axios_instance.post('/user/refresh', {
                'refreshToken': refreshToken
            }).then(response => {
                if (response.data.status) {
                    const token = response.data.response.jwtToken;
                    const refreshToken = response.data.response.refreshToken;

                    localStorage.setItem('token', token);
                    localStorage.setItem('refreshToken', refreshToken);

                    return {'token': token, 'refreshToken': refreshToken};
                }
            })
        }
    }
}

export default new AuthService();
