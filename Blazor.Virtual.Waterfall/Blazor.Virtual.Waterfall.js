let scrollContainer = null;
const findClosestScrollContainer = (element) => {
    if (!element || element === document.body || element === document.documentElement) {
        return null;
    }
    const style = getComputedStyle(element);
    if (style.overflowY !== 'visible') {
        return element;
    }
    return findClosestScrollContainer(element.parentElement);
};
const dispatcherObserversByDotNetIdPropname = Symbol();
const getObserversMapEntry = (dotNetHelper) => {
    var _a;
    const dotNetHelperDispatcher = dotNetHelper['_callDispatcher'];
    const dotNetHelperId = dotNetHelper['_id'];
    (_a = dotNetHelperDispatcher[dispatcherObserversByDotNetIdPropname]) !== null && _a !== void 0 ? _a : (dotNetHelperDispatcher[dispatcherObserversByDotNetIdPropname] = {});
    return {
        observersByDotNetObjectId: dotNetHelperDispatcher[dispatcherObserversByDotNetIdPropname],
        id: dotNetHelperId,
    };
};
const invokeInit = (dotNetHelper, container) => {
    const contentWidth = container.clientWidth;
    dotNetHelper.invokeMethodAsync("OnContentWidthChange", contentWidth, true);
};
const init = (dotNetHelper, spacerBefore, spacerAfter) => {
    const container = spacerBefore.parentElement;
    invokeInit(dotNetHelper, container);
    scrollContainer = findClosestScrollContainer(spacerBefore) || document.documentElement;
    const intersectionObserver = new IntersectionObserver(intersectionCallback, {
        root: scrollContainer,
        rootMargin: `${0}px`,
    });
    const resizeObserver = new ResizeObserver(resizeCallback);
    intersectionObserver.observe(spacerBefore);
    intersectionObserver.observe(spacerAfter);
    resizeObserver.observe(container);
    const mutationObserverBefore = createSpacerMutationObserver(spacerBefore);
    const mutationObserverAfter = createSpacerMutationObserver(spacerAfter);
    const { observersByDotNetObjectId, id } = getObserversMapEntry(dotNetHelper);
    observersByDotNetObjectId[id] = {
        intersectionObserver,
        resizeObserver,
        mutationObserverBefore,
        mutationObserverAfter,
    };
    function createSpacerMutationObserver(spacer) {
        const observerOptions = { attributes: true };
        const mutationObserver = new MutationObserver((mutations, observer) => {
            console.log(mutations);
            intersectionObserver.unobserve(spacer);
            intersectionObserver.observe(spacer);
        });
        mutationObserver.observe(spacer, observerOptions);
        return mutationObserver;
    }
    function intersectionCallback(entries) {
        entries.forEach((entry) => {
            console.log(entry);
            if (!entry.isIntersecting) {
                return;
            }
            const containerHeight = scrollContainer.clientHeight;
            const scrollTop = scrollContainer.scrollTop;
            const scrollHeight = scrollContainer.scrollHeight;
            if (entry.target === spacerBefore) {
                console.log(11111111);
                dotNetHelper.invokeMethodAsync('OnSpacerBeforeVisible', scrollTop, scrollHeight, containerHeight);
            }
            else if (entry.target === spacerAfter) {
                console.log(22222222);
                dotNetHelper.invokeMethodAsync('OnSpacerAfterVisible', scrollTop, scrollHeight, containerHeight);
            }
        });
    }
    function resizeCallback(entries) {
        entries.forEach((entry) => {
            if (entry.target == scrollContainer) {
                dotNetHelper.invokeMethodAsync("OnContentWidthChange", entry.contentRect.width, false);
            }
        });
    }
};
const scrollTo = (top) => {
    if (!scrollContainer) {
        scrollContainer.scrollTo({
            top: top,
            behavior: 'smooth',
        });
    }
};
const dispose = (dotNetHelper) => {
    const { observersByDotNetObjectId, id } = getObserversMapEntry(dotNetHelper);
    const observers = observersByDotNetObjectId[id];
    if (observers) {
        observers.intersectionObserver.disconnect();
        observers.resizeObserver.disconnect();
        observers.mutationObserverBefore.disconnect();
        observers.mutationObserverAfter.disconnect();
        dotNetHelper.dispose();
        delete observersByDotNetObjectId[id];
    }
};
const VirtualWaterfall = {
    init,
    scrollTo,
    dispose,
};
window['VirtualWaterfall'] = VirtualWaterfall;
export {};
//# sourceMappingURL=Blazor.Virtual.Waterfall.js.map