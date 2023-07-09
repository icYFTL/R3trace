import AuthService from "@/services/auth.service";

const token = localStorage.getItem('token');
const initialState = token
    ? {status: {loggedIn: true}, token}
    : {status: {loggedIn: false}, token: null};
export const auth = {
    namespaced: true,
    state: {
        ...initialState
    },
    getters: {
        isLoggedIn: state => state.loggedIn,
    },
    actions: {
        async login({commit}, data) {
            return await AuthService.login(data.username, data.password).then(
                token => {
                    commit('loginSuccess', token);
                    return Promise.resolve(token);
                },
                error => {
                    commit('loginFailure');
                    return Promise.reject(error);
                }
            );
        },
        async register({commit}, data) {
            return await AuthService.login(data.username, data.password).then(
                token => {
                    commit('registerSuccess', token);
                    return Promise.resolve(token);
                },
                error => {
                    commit('registerFailure');
                    return Promise.reject(error);
                }
            );
        },
        // async checkToken({commit}) {
        //     return AuthService.checkToken().then(x => {
        //         if (!x) {
        //             AuthService.logout();
        //             commit('logout');
        //         }
        //         return x;
        //     })
        // },
        async refresh() {
            let result = await AuthService.refreshToken();

            if (result !== undefined)
                return result;
            return null;
        },
        logout({commit}) {
            AuthService.logout();
            commit('logout');
        }
    },
    mutations: {
        loginSuccess(state, user) {
            state.status.loggedIn = true;
            state.user = user;
        },
        registerSuccess(state){
            // ?
        },
        registerFailure(state){
            // ?
        },
        loginFailure(state) {
            state.status.loggedIn = false;
            state.user = null;
        },
        logout(state) {
            state.status.loggedIn = false;
            state.user = null;
        }
    }
};
