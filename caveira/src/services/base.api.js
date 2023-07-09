import axios from "axios";
import store from "@/store/index";

export function getInstance() {
    const instance = axios.create({
        baseURL: import.meta.env.VITE_API_HOST
    });

    let token = localStorage.getItem('token');
    if (token)
        instance.defaults.headers.common['Authorization'] = `Bearer ${token}`

    instance.interceptors.response.use(
        response => response,
        async error => {
            const {status, config} = error.response;
            if (status === 401) {
                if (config.url === "/user/logout") {
                    // store.dispatch("auth/logout");
                    return Promise.reject(error);
                }

                let token;
                await store.dispatch("auth/refresh").then(x => {
                    token = x.token;
                }).catch(() => {
                });

                if (token == null) {
                    store.dispatch("auth/logout");
                    return Promise.reject();
                }

                instance.defaults.headers.common['Authorization'] = `Bearer ${token}`;

                config.headers['Authorization'] = `Bearer ${token}`;
                return instance.request(config);
            }
            if (status >= 500) {
                return Promise.reject();
            }

            store.dispatch("auth/logout");
            return Promise.reject(error);
        }
    );
    return instance;
}

