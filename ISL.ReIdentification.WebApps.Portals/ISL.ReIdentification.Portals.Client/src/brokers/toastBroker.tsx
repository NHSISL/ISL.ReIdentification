import React from "react"
import { ToastContainer, toast as rToast, ToastContent } from "react-toastify"

const ToastBroker = () => {}
const Container = () => <ToastContainer />

export const toastWarning = (content: ToastContent) => { return rToast.warn(content, { toastId: 1 }) };
export const toastError = (content: ToastContent) => { return rToast.error(content, { toastId: 2 }) };
export const toastInfo = (content: ToastContent) => { return rToast.info(content, { toastId: 3 }) };
export const toastSuccess = (content: ToastContent) => { return rToast.success(content, { toastId: 4 }) };

ToastBroker.Container = Container;
export default ToastBroker;