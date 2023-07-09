<template>
    <div>
        <div class="background-layer background-waves-animation"></div>
        <!--Todo: Make it center -->
        <div class="d-flex justify-content-center align-items-center">
            <div class="login-container p-5 rounded shadow">
                <h1 class="login-header text-center mb-4">Login</h1>
                <div>
                    <div class="form-group mb-3">
                        <label for="username">Username</label>
                        <input id="username" v-model="username" type="text" @keyup.enter="handleRegister"
                               class="form-control custom-input" required/>
                    </div>
                    <div class="form-group mb-3">
                        <label for="password">Password</label>
                        <input id="password" v-model="password" type="password" @keyup.enter="handleRegister"
                               class="form-control custom-input" required/>
                    </div>
                    <div class="form-group mb-3">
                        <label for="password">Confirm password</label>
                        <input id="confirm-password" v-model="confirm_password" type="password" @keyup.enter="handleRegister"
                               class="form-control custom-input" required/>
                    </div>
                    <div class="d-grid">
                        <button type="button" @click="handleRegister" class="btn btn-primary">Let me in</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>

import {useToast} from "vue-toastification";

export default {
    name: "RegisterView",
    data() {
        return {
            username: '',
            password: '',
            confirm_password: ''
        };
    },
    computed: {
        loggedIn() {
            return this.$store.state.auth.status.loggedIn;
        },
    },
    mounted() {

    },
    setup() {
        const toast = useToast()
        return {toast}
    },
    created() {
        if (this.loggedIn) {
            this.$router.push("/dashboard");
        } else {
            this.$store.dispatch("general/setHeaderVisibility", false);
            this.$store.dispatch("general/setMenuVisibility", false);
        }
    },
    methods: {
        onMouseMove(event) {
            const background = document.querySelector('.background-waves-animation');
            const x = event.clientX / window.innerWidth;
            const y = event.clientY / window.innerHeight;
            const angle = Math.atan2(y - 0.05, x - 0.05) * (180 / Math.PI);
            background.style.backgroundImage = `linear-gradient(${angle}deg, #6fbb26, #00dbff, #e19804)`;
        },
        handleRegister() {
            this.username = this.username.trim();
            if (this.username.length < 3) {//|| this.password.length < 8){
                this.toast.error("Empty login or password");
                return;
            }
            if (this.password !== this.confirm_password) {
                this.toast.error("Passwords do not match");
                return;
            }
            this.$store.dispatch("auth/register", {username: this.username, password: this.password}).then(
                () => {
                    this.$store.dispatch("auth/login", {username: this.username, password: this.password}).then(
                        () => {
                            this.$router.push("/dashboard");
                        },
                        () => {
                            this.toast.error("Invalid login or password")
                        }
                    );
                },
                () => {
                    this.toast.error("Something went wrong")
                }
            );

        },
    },
};
</script>
<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600&display=swap');

body {
    font-family: 'Poppins', sans-serif;
    background: linear-gradient(359deg, #6fbb26, #00dbff, #e19804);
    background-size: 400% 400%;
    animation: BackgroundWaves 50s ease infinite;
}

.login-container {
    background-color: rgba(255, 255, 255, 0.9);
    backdrop-filter: blur(5px);
    border-radius: 1rem;
    width: 100%;
    max-width: 400px;
    animation: fadeIn 1s;
    /*position: relative;*/
    overflow: hidden;
    border: 3px solid rgba(0, 0, 0, 0.1);
}

.login-header {
    font-weight: 500;
}

.background-layer {
    position: absolute;
    width: 100%;
    height: 100%;
    z-index: -9999;
    left: 0;
    top: 0;
}


.custom-input {
    border: none;
    border-bottom: 2px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    box-shadow: none;
    outline: none;
    transition: border-color 0.3s ease;
}

.custom-input:focus {
    border-color: #007bff;
}

.background-waves-animation {
    background: linear-gradient(359deg, #6fbb26, #00dbff, #e19804);
    background-size: 400% 400%;

    -webkit-animation: BackgroundWaves 50s ease infinite;
    -moz-animation: BackgroundWaves 50s ease infinite;
    animation: BackgroundWaves 50s ease infinite;
}
</style>
